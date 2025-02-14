# GettingStartedMassTransit

## Reference
https://github.com/MassTransit/Sample-GettingStarted

https://masstransit.io/quick-starts/rabbitmq

https://masstransit.io/quick-starts/in-memory

https://www.youtube.com/playlist?list=PLx8uyNNs1ri2MBx6BjPum5j9_MMdIfM9C

https://github.com/GabrieleTronchin/MassTransitPlayground

https://medium.com/bina-nusantara-it-division/a-beginners-guideline-to-rabbitmq-and-masstransit-part-2-implement-rabbitmq-in-code-with-af0503db2613

https://medium.com/@gabrieletronchin/c-net-8-exploring-different-types-of-producers-and-consumers-in-masstransit-ec7f782ab996

https://hamedsalameh.com/rabbitmq-and-masstransit-in-net-core-practical-guide/

https://github.com/MassTransit/MassTransit/tree/b5e0d0051eefcc392aeea10ab50eee79545fa73f/src/Persistence/MassTransit.MongoDbIntegration

## Configuration tiers messaging
### Docker RabbitMQ
locahost:15672

login : guest
Mode de passe : guest

### AWS SQS / SNS
Creation d'un user IAM au niveau de la sandbox AWS

Ajout de la policie SQS full access
Ajout de la policie SNS full access
ou
Ajout une policie custom
{
    "Version": "2012-10-17",
    "Statement": [
        {
            "Sid": "SqsAccess",
            "Effect": "Allow",
            "Action": [
                "sqs:SetQueueAttributes",
                "sqs:ReceiveMessage",
                "sqs:CreateQueue",
                "sqs:DeleteMessage",
                "sqs:SendMessage",
                "sqs:GetQueueUrl",
                "sqs:GetQueueAttributes",
                "sqs:ChangeMessageVisibility",
                "sqs:PurgeQueue",
                "sqs:DeleteQueue",
                "sqs:TagQueue"
            ],
            "Resource": "arn:aws:sqs:*:YOUR_ACCOUNT_ID:*"
        },{
            "Sid": "SnsAccess",
            "Effect": "Allow",
            "Action": [
                "sns:GetTopicAttributes",
                "sns:CreateTopic",
                "sns:Publish",
                "sns:Subscribe"
            ],
            "Resource": "arn:aws:sns:*:YOUR_ACCOUNT_ID:*"
        },{
            "Sid": "SnsListAccess",
            "Effect": "Allow",
            "Action": [
                "sns:ListTopics"
            ],
            "Resource": "*"
        }
    ]
}

Recup�ration des credentials

## Recherche
### Exemple de passage de donn�es g�n�riques dans un bus avec MassTransit
#### Exemple 1

Pour passer des donn�es g�n�riques dans un bus avec MassTransit, vous pouvez utiliser des messages g�n�riques en d�finissant des interfaces ou des classes g�n�riques. Voici un exemple de comment vous pouvez le faire :

1. **D�finir une Interface G�n�rique** :
   ```csharp
   public interface IGenericMessage<T>
   {
       T Data { get; }
   }
   ```

2. **Impl�menter l'Interface** :
   ```csharp
   public class GenericMessage<T> : IGenericMessage<T>
   {
       public T Data { get; private set; }

       public GenericMessage(T data)
       {
           Data = data;
       }
   }
   ```

3. **Envoyer un Message G�n�rique sur un queue sp�cifique** :
   ```csharp
   public async Task SendMessage<T>(IBusControl bus, T data)
   {
       var endpoint = await bus.GetSendEndpoint(new Uri("queue:generic-message-queue"));
       await endpoint.Send<IGenericMessage<T>>(new GenericMessage<T>(data));
   }
   ```

4. **Consommer un Message G�n�rique** :
   ```csharp
   public class GenericMessageConsumer<T> : IConsumer<IGenericMessage<T>>
   {
       public Task Consume(ConsumeContext<IGenericMessage<T>> context)
       {
           var data = context.Message.Data;
           // Traitez les donn�es ici
           return Task.CompletedTask;
       }
   }
   ```

5. **Configurer le Bus et les Consommateurs pour conssomer les messages sur une queue sp�cifique** :
   ```csharp
   public void ConfigureServices(IServiceCollection services)
   {
       services.AddMassTransit(x =>
       {
           x.AddConsumer<GenericMessageConsumer<string>>();
           x.UsingRabbitMq((context, cfg) =>
           {
               cfg.ReceiveEndpoint("generic-message-queue", e =>
               {
                   e.ConfigureConsumer<GenericMessageConsumer<string>>(context);
               });
           });
       });
   }
   ```

Avec cette approche, vous pouvez envoyer et consommer des messages contenant des donn�es de types diff�rents en utilisant une seule infrastructure de message g�n�rique[1](https://markgossa.com/2022/06/masstransit-and-mediatr.html)[2](https://github.com/MassTransit/MassTransit/discussions/3694).

Si vous avez des questions sp�cifiques ou des cas d'utilisation particuliers, n'h�sitez pas � les partager !

[1](https://markgossa.com/2022/06/masstransit-and-mediatr.html): MassTransit Documentation
[2](https://github.com/MassTransit/MassTransit/discussions/3694): [GitHub Discussion on Generic Messages](https://github.com/MassTransit/MassTransit/discussions/3694)

#### Exemple 2
Pour passer des donn�es g�n�riques dans un bus avec MassTransit en utilisant Amazon SQS, vous pouvez suivre ces �tapes :

1. **D�finir une Interface G�n�rique** :
   ```csharp
   public interface IGenericMessage<T>
   {
       T Data { get; }
   }
   ```

2. **Impl�menter l'Interface** :
   ```csharp
   public class GenericMessage<T> : IGenericMessage<T>
   {
       public T Data { get; private set; }

       public GenericMessage(T data)
       {
           Data = data;
       }
   }
   ```

3. **Configurer MassTransit avec Amazon SQS** :
   ```csharp
   public void ConfigureServices(IServiceCollection services)
   {
       services.AddMassTransit(x =>
       {
           x.AddConsumer<GenericMessageConsumer<string>>();
           x.UsingAmazonSqs((context, cfg) =>
           {
               cfg.Host("us-east-2", h =>
               {
                   h.AccessKey("your-iam-access-key");
                   h.SecretKey("your-iam-secret-key");
               });

               cfg.ReceiveEndpoint("generic-message-queue", e =>
               {
                   e.ConfigureConsumer<GenericMessageConsumer<string>>(context);
               });
           });
       });
   }
   ```

4. **Envoyer un Message G�n�rique** :
   ```csharp
   public async Task SendMessage<T>(IBusControl bus, T data)
   {
       var endpoint = await bus.GetSendEndpoint(new Uri("queue:generic-message-queue"));
       await endpoint.Send<IGenericMessage<T>>(new GenericMessage<T>(data));
   }
   ```

5. **Consommer un Message G�n�rique** :
   ```csharp
   public class GenericMessageConsumer<T> : IConsumer<IGenericMessage<T>>
   {
       public Task Consume(ConsumeContext<IGenericMessage<T>> context)
       {
           var data = context.Message.Data;
           // Traitez les donn�es ici
           return Task.CompletedTask;
       }
   }
   ```

Avec cette configuration, vous pouvez envoyer et consommer des messages g�n�riques via Amazon SQS en utilisant MassTransit[1](https://masstransit.io/documentation/configuration/transports/amazon-sqs)[2](https://www.milanjovanovic.tech/blog/complete-guide-to-amazon-sqs-and-amazon-sns-with-masstransit).

Si vous avez des questions sp�cifiques ou des cas d'utilisation particuliers, n'h�sitez pas � les partager !

[1](https://masstransit.io/documentation/configuration/transports/amazon-sqs): [MassTransit Documentation](https://masstransit.io/documentation/configuration/transports/amazon-sqs)
[2](https://www.milanjovanovic.tech/blog/complete-guide-to-amazon-sqs-and-amazon-sns-with-masstransit): [Complete Guide to Amazon SQS and Amazon SNS With MassTransit](https://www.milanjovanovic.tech/blog/complete-guide-to-amazon-sqs-and-amazon-sns-with-masstransit)

https://github.com/davidbytyqi1/DotNETRabbitMQ/blob/master/QueueSenderService/Controllers/QueueSenderController.cs
https://medium.com/@david.bytyqi/net-core-8-with-rabbitmq-and-masstransit-77899c27fd79

### Protocole d'�change de data tel que JSON mais qui garde le type de donn�e : Protobuf

* Protocol Buffers (Protobuf) :
    Protocole de s�rialisation d�velopp� par Google.
    Il permet de d�crire des structures de donn�es (types, champs) dans un fichier .proto.
    Il est plus compact et plus rapide que JSON et XML.
    Les types sont explicitement d�finis dans le sch�ma, ce qui permet de maintenir une forte typage lors de la s�rialisation et de la d�s�rialisation.
    https://medium.com/@hanxuyang0826/using-protocol-buffers-protobuf-in-net-e152f75b77ae
    https://github.com/protobuf-net/protobuf-net
* MessagePack :
	Format de s�rialisation binaire qui est plus compact et plus rapide que JSON.
	Il est plus rapide que JSON et plus compact que BSON.
	Il est facile � utiliser et � int�grer dans les applications .NET.
	https://messagepack.org/
* Avro :
	Format de s�rialisation de donn�es d�velopp� par Apache.
	Il est compact, rapide et prend en charge l'�volution des sch�mas.
	Il est utilis� dans des syst�mes distribu�s comme Apache Kafka.
	https://avro.apache.org/
  https://github.com/AdrianStrugala/AvroConvert

### Format d'�change g�r� par Mass transit et int�gr� � dotnet : Bson
Utilis� du bson pour la s�rialisation des messages dans MassTransit.

```csharp
services.AddMassTransit(x =>
{
    x.UsingAmazonSqs((context, cfg) =>
    {
        cfg.Host("us-east-2", h =>
        {
            h.AccessKey("votre-access-key");
            h.SecretKey("votre-secret-key");
        });
        cfg.UseBsonSerializer();
    });
});
```

Ainsi on doit pouvoir r�cup�rer du bson quand on consume le message?

Pour ajouter un objet BSON dans une base de donn�es MongoDB, vous pouvez utiliser le pilote MongoDB pour C#. Voici un exemple de code pour ins�rer un document BSON dans une collection MongoDB :

1. **Installer le package NuGet** : Assurez-vous d'avoir install� le package `MongoDB.Driver`.

```bash
dotnet add package MongoDB.Driver
```

2. **Cr�er et ins�rer un document BSON** :

```csharp
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

public class Program
{
    public static async Task Main(string[] args)
    {
        // Connexion � la base de donn�es MongoDB
        var client = new MongoClient("mongodb://localhost:27017");
        var database = client.GetDatabase("votre_base_de_donnees");
        var collection = database.GetCollection<BsonDocument>("votre_collection");

        // Cr�ation d'un document BSON
        var document = new BsonDocument
        {
            { "nom", "John Doe" },
            { "age", 30 },
            { "adresse", new BsonDocument
                {
                    { "rue", "123 Rue Principale" },
                    { "ville", "Paris" },
                    { "code_postal", "75001" }
                }
            }
        };

        // Insertion du document dans la collection
        await collection.InsertOneAsync(document);

        Console.WriteLine("Document ins�r� avec succ�s !");
    }
}
```

Ce code se connecte � une base de donn�es MongoDB, cr�e un document BSON et l'ins�re dans une collection sp�cifi�e. Assurez-vous de remplacer `"mongodb://localhost:27017"`, `"votre_base_de_donnees"`, et `"votre_collection"` par les valeurs appropri�es pour votre configuration.

https://learn.microsoft.com/fr-fr/aspnet/web-api/overview/formats-and-model-binding/bson-support-in-web-api-21

https://masstransit.io/documentation/configuration/serialization

**TODO** : Checker la bon typage des donn�es de cette objet mongodb une fois ins�rer en base de donn�es

### Configuration avec Mass transit et une seule queue

Oui, bien s�r ! Voici un exemple d'utilisation de MassTransit avec une seule queue mais plusieurs types de messages :

1. **Configuration de MassTransit** :
   Vous configurez MassTransit pour utiliser une seule queue et plusieurs consommateurs pour diff�rents types de messages.
   ```csharp
   services.AddMassTransit(x =>
   {
       x.AddConsumer<MessageAConsumer>();
       x.AddConsumer<MessageBConsumer>();

       x.UsingRabbitMq((context, cfg) =>
       {
           cfg.Host("rabbitmq://localhost");

           cfg.ReceiveEndpoint("single_queue", e =>
           {
               e.ConfigureConsumer<MessageAConsumer>(context);
               e.ConfigureConsumer<MessageBConsumer>(context);
           });
       });
   });
   ```

2. **D�finition des messages** :
   Vous d�finissez les diff�rents types de messages qui seront envoy�s � la queue.
   ```csharp
   public class MessageA
   {
       public string PropertyA { get; set; }
   }

   public class MessageB
   {
       public string PropertyB { get; set; }
   }
   ```

3. **Cr�ation des consommateurs** :
   Vous cr�ez des consommateurs pour chaque type de message.
   ```csharp
   public class MessageAConsumer : IConsumer<MessageA>
   {
       public Task Consume(ConsumeContext<MessageA> context)
       {
           Console.WriteLine($"Received MessageA: {context.Message.PropertyA}");
           return Task.CompletedTask;
       }
   }

   public class MessageBConsumer : IConsumer<MessageB>
   {
       public Task Consume(ConsumeContext<MessageB> context)
       {
           Console.WriteLine($"Received MessageB: {context.Message.PropertyB}");
           return Task.CompletedTask;
       }
   }
   ```

4. **Envoi des messages** :
   Vous pouvez envoyer les diff�rents types de messages � la queue.
   ```csharp
   public class MessageSender
   {
       private readonly IBus _bus;

       public MessageSender(IBus bus)
       {
           _bus = bus;
       }

       public async Task SendMessages()
       {
           await _bus.Publish(new MessageA { PropertyA = "ValueA" });
           await _bus.Publish(new MessageB { PropertyB = "ValueB" });
       }
   }
   ```

Avec cette configuration, MassTransit utilisera une seule queue (`single_queue`) pour recevoir les messages de types `MessageA` et `MessageB`, et les acheminera vers les consommateurs appropri�s (`MessageAConsumer` et `MessageBConsumer`).

Est-ce que cela r�pond � votre question ? Avez-vous besoin de plus de d�tails sur un aspect particulier ?

### Serialisation via un JsonObject

`JsonObject` et `JsonNode` sont tous deux des composants de la biblioth�que `System.Text.Json` en .NET, mais ils ont des r�les et des utilisations l�g�rement diff�rents :

1. **JsonNode** :
   - **Classe de base abstraite** : `JsonNode` est une classe de base abstraite qui repr�sente un n�ud dans un document JSON. Elle peut �tre d�riv�e en `JsonObject`, `JsonArray`, ou `JsonValue`[1](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.nodes.jsonobject?view=net-9.0).
   - **Flexibilit�** : `JsonNode` offre une flexibilit� pour manipuler des structures JSON dynamiques sans conna�tre � l'avance leur type exact. Vous pouvez utiliser des m�thodes comme `AsObject()`, `AsArray()`, et `AsValue()` pour convertir un `JsonNode` en son type d�riv� sp�cifique[2](https://dev.to/vparab/working-with-jsonobject-jsonnode-jsonvalue-and-jsonarray-systemtextjson-api-5b8l).

2. **JsonObject** :
   - **Repr�sentation d'un objet JSON** : `JsonObject` est une classe d�riv�e de `JsonNode` qui repr�sente sp�cifiquement un objet JSON, c'est-�-dire une collection de paires cl�-valeur[1](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.nodes.jsonobject?view=net-9.0).
   - **Interface de dictionnaire** : `JsonObject` impl�mente les interfaces `IDictionary<string, JsonNode>` et `ICollection<KeyValuePair<string, JsonNode>>`, ce qui permet de manipuler les propri�t�s de l'objet JSON comme un dictionnaire[1](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.nodes.jsonobject?view=net-9.0).
   - **Mutabilit�** : `JsonObject` permet de modifier les propri�t�s de l'objet JSON apr�s sa cr�ation, ce qui est utile pour construire ou mettre � jour des objets JSON de mani�re dynamique[3](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/use-dom).

Voici un exemple pour illustrer la diff�rence :
```csharp
using System;
using System.Text.Json.Nodes;

public class Example
{
    public static void Main()
    {
        // Cr�ation d'un JsonObject
        var jsonObject = new JsonObject
        {
            ["name"] = "John Doe",
            ["age"] = 30,
            ["isEmployed"] = true
        };

        // Acc�s aux propri�t�s via JsonObject
        Console.WriteLine(jsonObject["name"]); // John Doe

        // Utilisation de JsonNode pour la flexibilit�
        JsonNode jsonNode = jsonObject;
        if (jsonNode is JsonObject obj)
        {
            Console.WriteLine(obj["age"]); // 30
        }
    }
}
```

En r�sum�, `JsonObject` est une sp�cialisation de `JsonNode` pour les objets JSON, offrant des fonctionnalit�s sp�cifiques pour manipuler des paires cl�-valeur, tandis que `JsonNode` fournit une base flexible pour travailler avec diff�rents types de n�uds JSON.

Les sc�narios d'utilisation typiques pour `JsonObject` et `JsonNode` en .NET incluent plusieurs situations o� la manipulation de donn�es JSON est n�cessaire. Voici quelques exemples :

1. **Manipulation dynamique de JSON** :
   - **JsonNode** : Id�al pour les sc�narios o� la structure JSON est inconnue � l'avance ou peut changer. Par exemple, lors de la lecture de donn�es JSON provenant d'une API externe o� les propri�t�s peuvent varier.
   - **JsonObject** : Utilis� lorsque vous savez que vous travaillez avec un objet JSON et que vous souhaitez acc�der ou modifier ses propri�t�s de mani�re structur�e.

2. **Construction de documents JSON** :
   - **JsonObject** : Pratique pour construire des objets JSON de mani�re programmatique, en ajoutant des paires cl�-valeur dynamiquement.
   ```csharp
   var jsonObject = new JsonObject
   {
       ["name"] = "John Doe",
       ["age"] = 30,
       ["isEmployed"] = true
   };
   ```

3. **Parsing et navigation dans des documents JSON** :
   - **JsonNode** : Permet de parcourir et de naviguer dans des documents JSON complexes, en acc�dant aux n�uds enfants de mani�re flexible.
   ```csharp
   JsonNode jsonNode = JsonNode.Parse(jsonString);
   string name = jsonNode["name"].GetValue<string>();
   ```

4. **Validation et transformation de donn�es** :
   - **JsonObject** : Utilis� pour valider et transformer des objets JSON avant de les utiliser dans l'application. Par exemple, v�rifier la pr�sence de certaines propri�t�s ou transformer les valeurs.
   ```csharp
   if (jsonObject.ContainsKey("age"))
   {
       int age = jsonObject["age"].GetValue<int>();
       // Transformation ou validation de l'�ge
   }
   ```

5. **Interop�rabilit� avec des syst�mes externes** :
   - **JsonNode** et **JsonObject** : Utilis�s pour interagir avec des services web, des bases de donn�es NoSQL, ou d'autres syst�mes qui utilisent JSON comme format d'�change de donn�es.

6. **S�rialisation et d�s�rialisation** :
   - **JsonObject** : Utilis� pour d�s�rialiser des cha�nes JSON en objets fortement typ�s, puis les manipuler et les res�rialiser si n�cessaire.
   ```csharp
   string jsonString = jsonObject.ToString();
   ```

Ces sc�narios montrent comment `JsonObject` et `JsonNode` peuvent �tre utilis�s pour manipuler des donn�es JSON de mani�re flexible et efficace dans diff�rentes situations.

### Sauvegarde du type de chaque �l�ment dans un JsonObject
Oui, il est possible de sauvegarder le type de chaque �l�ment dans un `JsonObject` en ajoutant des m�tadonn�es sur les types. Une approche courante consiste � inclure des informations sur le type dans les propri�t�s de l'objet JSON. Voici un exemple de comment vous pouvez le faire :

1. **Ajouter des m�tadonn�es de type** :
   Vous pouvez ajouter une propri�t� suppl�mentaire pour chaque �l�ment qui indique son type.
   ```csharp
   var jsonObject = new JsonObject
   {
       ["name"] = "John Doe",
       ["name_type"] = "string",
       ["age"] = 30,
       ["age_type"] = "int",
       ["isEmployed"] = true,
       ["isEmployed_type"] = "bool"
   };
   ```

2. **S�rialisation et d�s�rialisation avec types** :
   Lors de la d�s�rialisation, vous pouvez lire ces m�tadonn�es pour d�terminer le type de chaque propri�t�.
   ```csharp
   using System;
   using System.Text.Json;
   using System.Text.Json.Nodes;

   public class Example
   {
       public static void Main()
       {
           var jsonObject = new JsonObject
           {
               ["name"] = "John Doe",
               ["name_type"] = "string",
               ["age"] = 30,
               ["age_type"] = "int",
               ["isEmployed"] = true,
               ["isEmployed_type"] = "bool"
           };

           string jsonString = jsonObject.ToString();
           Console.WriteLine(jsonString);

           JsonNode parsedNode = JsonNode.Parse(jsonString);
           var parsedObject = parsedNode as JsonObject;

           foreach (var property in parsedObject)
           {
               if (property.Key.EndsWith("_type"))
               {
                   string originalKey = property.Key.Replace("_type", "");
                   string type = property.Value.GetValue<string>();

                   Console.WriteLine($"Property: {originalKey}, Type: {type}");
               }
           }
       }
   }
   ```

3. **Utilisation d'un wrapper pour les types** :
   Une autre approche consiste � cr�er une classe wrapper qui inclut � la fois la valeur et le type de chaque propri�t�.
   ```csharp
   public class TypedProperty
   {
       public object Value { get; set; }
       public string Type { get; set; }
   }

   var jsonObject = new JsonObject
   {
       ["name"] = new JsonObject
       {
           ["value"] = "John Doe",
           ["type"] = "string"
       },
       ["age"] = new JsonObject
       {
           ["value"] = 30,
           ["type"] = "int"
       },
       ["isEmployed"] = new JsonObject
       {
           ["value"] = true,
           ["type"] = "bool"
       }
   };
   ```

Ces m�thodes vous permettent de sauvegarder et de r�cup�rer les types de chaque �l�ment dans un `JsonObject`, ce qui peut �tre utile pour des sc�narios o� vous devez conserver des informations de type dynamiques.

### S�rialisation de la classe TypedProperty en JSON
La s�rialisation de la classe `TypedProperty` en JSON implique la conversion d'une instance de cette classe en une cha�ne JSON. Voici comment vous pouvez le faire en utilisant `System.Text.Json` :

1. **D�finir la classe `TypedProperty`** :
   Cette classe contient deux propri�t�s : `Value` pour la valeur de la propri�t� et `Type` pour le type de la propri�t�.
   ```csharp
   public class TypedProperty
   {
       public object Value { get; set; }
       public string Type { get; set; }
   }
   ```

2. **Cr�er une instance de `TypedProperty`** :
   Vous pouvez cr�er des instances de cette classe pour diff�rentes propri�t�s avec leurs valeurs et types respectifs.
   ```csharp
   var nameProperty = new TypedProperty
   {
       Value = "John Doe",
       Type = "string"
   };

   var ageProperty = new TypedProperty
   {
       Value = 30,
       Type = "int"
   };

   var isEmployedProperty = new TypedProperty
   {
       Value = true,
       Type = "bool"
   };
   ```

3. **S�rialiser les instances en JSON** :
   Utilisez `JsonSerializer` pour convertir les instances de `TypedProperty` en cha�nes JSON.
   ```csharp
   using System;
   using System.Text.Json;

   public class Example
   {
       public static void Main()
       {
           var nameProperty = new TypedProperty
           {
               Value = "John Doe",
               Type = "string"
           };

           var ageProperty = new TypedProperty
           {
               Value = 30,
               Type = "int"
           };

           var isEmployedProperty = new TypedProperty
           {
               Value = true,
               Type = "bool"
           };

           string nameJson = JsonSerializer.Serialize(nameProperty);
           string ageJson = JsonSerializer.Serialize(ageProperty);
           string isEmployedJson = JsonSerializer.Serialize(isEmployedProperty);

           Console.WriteLine(nameJson);
           Console.WriteLine(ageJson);
           Console.WriteLine(isEmployedJson);
       }
   }
   ```

4. **R�sultat de la s�rialisation** :
   Les instances de `TypedProperty` seront s�rialis�es en JSON avec leurs valeurs et types respectifs.
   ```json
   {
       "Value": "John Doe",
       "Type": "string"
   }
   {
       "Value": 30,
       "Type": "int"
   }
   {
       "Value": true,
       "Type": "bool"
   }
   ```

5. **D�s�rialisation** :
   Pour d�s�rialiser ces cha�nes JSON en instances de `TypedProperty`, vous pouvez utiliser `JsonSerializer.Deserialize`.
   ```csharp
   var deserializedNameProperty = JsonSerializer.Deserialize<TypedProperty>(nameJson);
   var deserializedAgeProperty = JsonSerializer.Deserialize<TypedProperty>(ageJson);
   var deserializedIsEmployedProperty = JsonSerializer.Deserialize<TypedProperty>(isEmployedJson);

   Console.WriteLine($"Name: {deserializedNameProperty.Value}, Type: {deserializedNameProperty.Type}");
   Console.WriteLine($"Age: {deserializedAgeProperty.Value}, Type: {deserializedAgeProperty.Type}");
   Console.WriteLine($"Is Employed: {deserializedIsEmployedProperty.Value}, Type: {deserializedIsEmployedProperty.Type}");
   ```

Cette approche vous permet de s�rialiser et d�s�rialiser des propri�t�s avec leurs types, ce qui peut �tre utile pour des sc�narios o� vous devez conserver des informations de type dynamiques.

## Soluton de serialisation et d�s�rialisation de donn�es en passant le type de donn�es dans le message

### Fonctionnement

Au niveau du publisher, on peut envoyer un message de type `ApplicationDeltaEntity` ou `ApplicationTetaEntity` en utilisant le endpoint `POST /api/audittrailevent/publish`

La partie commune de change jamais, seul la partie Data varie

```csharp
public record BaseEvent
{
    [BsonGuidRepresentation(MongoDB.Bson.GuidRepresentation.Standard)]
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime CreationDate { get; set; } = DateTime.UtcNow;
}

public record AuditTrailEvent : BaseEvent
{
    public required DateTime EventTime { get; set; }
    public required string EventSource { get; set; }
    public required string EventType { get; set; }
    public required string Domain { get; set; }
    public UserInfoEntity? User { get; set; }
    [BsonIgnore]
    public string Data { get; set; }
}
```

L'EventType de la structure en json doit correspondre � la structure choisi pour la partie propre � chaque businnes app (Admin, Reserving) : la partie serialiser en json Data.

### ApplicationDeltaEntity en json

#### via le Publisher

eventType : `GettingStartedMassTransit.Common.EventBus.Entity.Application.ApplicationDeltaEntity`

```json
{
  "creationDate": "2025-02-12T15:44:14.852Z",
  "eventTime": "2025-02-12T15:44:14.852Z",
  "eventSource": "event source",
  "eventType": "GettingStartedMassTransit.Common.EventBus.Entity.Application.ApplicationDeltaEntity",
  "domain": "Delta",
  "user": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "firstName": "user",
    "lastName": "test"
  },
  "data": "{\"Description\": \"This is a description\", \"Name\": \"Sample Name\"}"
}
```

#### via le consumer

On stocke bien le bson dans la base de donn�es MongoDB

```bson
{
  "_id": {
    "$binary": {
      "base64": "SNLVI5I7Su2oZkjsbBcETQ==",
      "subType": "04"
    }
  },
  "CreationDate": {
    "$date": "2025-02-13T15:44:14.852Z"
  },
  "EventTime": {
    "$date": "2025-02-13T15:44:14.852Z"
  },
  "EventSource": "event source",
  "EventType": "GettingStartedMassTransit.Common.EventBus.Entity.Application.ApplicationDeltaEntity",
  "Domain": "Delta",
  "User": {
    "_id": {
      "$binary": {
        "base64": "P6hfZFcXRWKz/CyWP2avpg==",
        "subType": "04"
      }
    },
    "FirstName": "user",
    "LastName": "test"
  },
  "data": {
    "Description": "This is another description",
    "Name": "Another Sample Name"
  }
}
```

### ApplicationTetaEntity en json

#### via le Publisher
eventType : `GettingStartedMassTransit.Common.EventBus.Entity.Application.ApplicationTetaEntity`

```json
{
  "creationDate": "2025-02-13T15:44:14.852Z",
  "eventTime": "2025-02-13T15:44:14.852Z",
  "eventSource": "event source",
  "eventType": "GettingStartedMassTransit.Common.EventBus.Entity.Application.ApplicationTetaEntity",
  "domain": "Teta",
  "user": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "firstName": "user",
    "lastName": "test"
  },
  "data": "{\"Description\": \"Lorem ipsum\", \"Time\": \"12:34:56\", \"Study\": { \"Id\": \"d4e3a2b1-c0b9-4e8f-9d7c-6b5a4f3e2d1a\", \"Name\": \"Study Name\", \"Date\": \"2022-01-01T00:00:00\", \"HourDuration\": 2,\"floatNumber\": 3.14,\"doubleNumber\": 3.14159,\"decimalNumber\": 123.45,\"IsActive\": true}}"
}
```

#### Via le Consumer

On stocke bien le bson dans la base de donn�es MongoDB. Les donn�es sont correctement typ�es.

```bson
{
  "_id": {
    "$binary": {
      "base64": "x7tZtwGHRTuQGAMrmZUbyg==",
      "subType": "04"
    }
  },
  "CreationDate": {
    "$date": "2025-02-13T15:44:14.852Z"
  },
  "EventTime": {
    "$date": "2025-02-13T15:44:14.852Z"
  },
  "EventSource": "event source",
  "EventType": "GettingStartedMassTransit.Common.EventBus.Entity.Application.ApplicationTetaEntity",
  "Domain": "Teta",
  "User": {
    "_id": {
      "$binary": {
        "base64": "P6hfZFcXRWKz/CyWP2avpg==",
        "subType": "04"
      }
    },
    "FirstName": "user",
    "LastName": "test"
  },
  "data": {
    "Description": "Lorem ipsum",
    "Time": {
      "$numberLong": "452960000000"
    },
    "Study": {
      "_id": {
        "$binary": {
          "base64": "1OOiscC5To+dfGtaTz4tGg==",
          "subType": "04"
        }
      },
      "Name": "Study Name",
      "Date": {
        "$date": "2021-12-31T23:00:00.000Z"
      },
      "HourDuration": 2,
      "floatNumber": 3.140000104904175,
      "doubleNumber": 3.14159,
      "decimalNumber": {
        "$numberDecimal": "123.45"
      },
      "IsActive": true
    }
  }
}
```