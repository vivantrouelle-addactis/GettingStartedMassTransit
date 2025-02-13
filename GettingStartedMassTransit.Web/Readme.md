# Reference
https://github.com/MassTransit/Sample-GettingStarted

https://masstransit.io/quick-starts/rabbitmq

https://masstransit.io/quick-starts/in-memory

# Docker RabbitMQ
locahost:15672

login : guest
Mode de passe : guest

# AWS
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

Recupération des credentials

# Recherche
## Exemple de passage de données génériques dans un bus avec MassTransit
### Exemple 1

Pour passer des données génériques dans un bus avec MassTransit, vous pouvez utiliser des messages génériques en définissant des interfaces ou des classes génériques. Voici un exemple de comment vous pouvez le faire :

1. **Définir une Interface Générique** :
   ```csharp
   public interface IGenericMessage<T>
   {
       T Data { get; }
   }
   ```

2. **Implémenter l'Interface** :
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

3. **Envoyer un Message Générique sur un queue spécifique** :
   ```csharp
   public async Task SendMessage<T>(IBusControl bus, T data)
   {
       var endpoint = await bus.GetSendEndpoint(new Uri("queue:generic-message-queue"));
       await endpoint.Send<IGenericMessage<T>>(new GenericMessage<T>(data));
   }
   ```

4. **Consommer un Message Générique** :
   ```csharp
   public class GenericMessageConsumer<T> : IConsumer<IGenericMessage<T>>
   {
       public Task Consume(ConsumeContext<IGenericMessage<T>> context)
       {
           var data = context.Message.Data;
           // Traitez les données ici
           return Task.CompletedTask;
       }
   }
   ```

5. **Configurer le Bus et les Consommateurs pour conssomer les messages sur une queue spécifique** :
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

Avec cette approche, vous pouvez envoyer et consommer des messages contenant des données de types différents en utilisant une seule infrastructure de message générique[1](https://markgossa.com/2022/06/masstransit-and-mediatr.html)[2](https://github.com/MassTransit/MassTransit/discussions/3694).

Si vous avez des questions spécifiques ou des cas d'utilisation particuliers, n'hésitez pas à les partager !

[1](https://markgossa.com/2022/06/masstransit-and-mediatr.html): MassTransit Documentation
[2](https://github.com/MassTransit/MassTransit/discussions/3694): [GitHub Discussion on Generic Messages](https://github.com/MassTransit/MassTransit/discussions/3694)

### Exemple 2
Pour passer des données génériques dans un bus avec MassTransit en utilisant Amazon SQS, vous pouvez suivre ces étapes :

1. **Définir une Interface Générique** :
   ```csharp
   public interface IGenericMessage<T>
   {
       T Data { get; }
   }
   ```

2. **Implémenter l'Interface** :
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

4. **Envoyer un Message Générique** :
   ```csharp
   public async Task SendMessage<T>(IBusControl bus, T data)
   {
       var endpoint = await bus.GetSendEndpoint(new Uri("queue:generic-message-queue"));
       await endpoint.Send<IGenericMessage<T>>(new GenericMessage<T>(data));
   }
   ```

5. **Consommer un Message Générique** :
   ```csharp
   public class GenericMessageConsumer<T> : IConsumer<IGenericMessage<T>>
   {
       public Task Consume(ConsumeContext<IGenericMessage<T>> context)
       {
           var data = context.Message.Data;
           // Traitez les données ici
           return Task.CompletedTask;
       }
   }
   ```

Avec cette configuration, vous pouvez envoyer et consommer des messages génériques via Amazon SQS en utilisant MassTransit[1](https://masstransit.io/documentation/configuration/transports/amazon-sqs)[2](https://www.milanjovanovic.tech/blog/complete-guide-to-amazon-sqs-and-amazon-sns-with-masstransit).

Si vous avez des questions spécifiques ou des cas d'utilisation particuliers, n'hésitez pas à les partager !

[1](https://masstransit.io/documentation/configuration/transports/amazon-sqs): [MassTransit Documentation](https://masstransit.io/documentation/configuration/transports/amazon-sqs)
[2](https://www.milanjovanovic.tech/blog/complete-guide-to-amazon-sqs-and-amazon-sns-with-masstransit): [Complete Guide to Amazon SQS and Amazon SNS With MassTransit](https://www.milanjovanovic.tech/blog/complete-guide-to-amazon-sqs-and-amazon-sns-with-masstransit)

https://github.com/davidbytyqi1/DotNETRabbitMQ/blob/master/QueueSenderService/Controllers/QueueSenderController.cs
https://medium.com/@david.bytyqi/net-core-8-with-rabbitmq-and-masstransit-77899c27fd79

## Protocole d'échange de data tel que JSON mais qui garde le type de donnée : Protobuf

* Protocol Buffers (Protobuf) :
    Protocole de sérialisation développé par Google.
    Il permet de décrire des structures de données (types, champs) dans un fichier .proto.
    Il est plus compact et plus rapide que JSON et XML.
    Les types sont explicitement définis dans le schéma, ce qui permet de maintenir une forte typage lors de la sérialisation et de la désérialisation.
    https://medium.com/@hanxuyang0826/using-protocol-buffers-protobuf-in-net-e152f75b77ae
    https://github.com/protobuf-net/protobuf-net
* MessagePack :
	Format de sérialisation binaire qui est plus compact et plus rapide que JSON.
	Il est plus rapide que JSON et plus compact que BSON.
	Il est facile à utiliser et à intégrer dans les applications .NET.
	https://messagepack.org/
* Avro :
	Format de sérialisation de données développé par Apache.
	Il est compact, rapide et prend en charge l'évolution des schémas.
	Il est utilisé dans des systèmes distribués comme Apache Kafka.
	https://avro.apache.org/
  https://github.com/AdrianStrugala/AvroConvert

## Format d'échange géré par Mass transit et intégré à dotnet : Bson
Utilisé du bson pour la sérialisation des messages dans MassTransit.

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

Ainsi on doit pouvoir récupérer du bson quand on consume le message?

Pour ajouter un objet BSON dans une base de données MongoDB, vous pouvez utiliser le pilote MongoDB pour C#. Voici un exemple de code pour insérer un document BSON dans une collection MongoDB :

1. **Installer le package NuGet** : Assurez-vous d'avoir installé le package `MongoDB.Driver`.

```bash
dotnet add package MongoDB.Driver
```

2. **Créer et insérer un document BSON** :

```csharp
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

public class Program
{
    public static async Task Main(string[] args)
    {
        // Connexion à la base de données MongoDB
        var client = new MongoClient("mongodb://localhost:27017");
        var database = client.GetDatabase("votre_base_de_donnees");
        var collection = database.GetCollection<BsonDocument>("votre_collection");

        // Création d'un document BSON
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

        Console.WriteLine("Document inséré avec succès !");
    }
}
```

Ce code se connecte à une base de données MongoDB, crée un document BSON et l'insère dans une collection spécifiée. Assurez-vous de remplacer `"mongodb://localhost:27017"`, `"votre_base_de_donnees"`, et `"votre_collection"` par les valeurs appropriées pour votre configuration.

https://learn.microsoft.com/fr-fr/aspnet/web-api/overview/formats-and-model-binding/bson-support-in-web-api-21

https://masstransit.io/documentation/configuration/serialization

**TODO** : Checker la bon typage des données de cette objet mongodb une fois insérer en base de données

## Configuration avec Mass transit et une seule queue

Oui, bien sûr ! Voici un exemple d'utilisation de MassTransit avec une seule queue mais plusieurs types de messages :

1. **Configuration de MassTransit** :
   Vous configurez MassTransit pour utiliser une seule queue et plusieurs consommateurs pour différents types de messages.
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

2. **Définition des messages** :
   Vous définissez les différents types de messages qui seront envoyés à la queue.
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

3. **Création des consommateurs** :
   Vous créez des consommateurs pour chaque type de message.
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
   Vous pouvez envoyer les différents types de messages à la queue.
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

Avec cette configuration, MassTransit utilisera une seule queue (`single_queue`) pour recevoir les messages de types `MessageA` et `MessageB`, et les acheminera vers les consommateurs appropriés (`MessageAConsumer` et `MessageBConsumer`).

Est-ce que cela répond à votre question ? Avez-vous besoin de plus de détails sur un aspect particulier ?

## Serialisation via un JsonObject

`JsonObject` et `JsonNode` sont tous deux des composants de la bibliothèque `System.Text.Json` en .NET, mais ils ont des rôles et des utilisations légèrement différents :

1. **JsonNode** :
   - **Classe de base abstraite** : `JsonNode` est une classe de base abstraite qui représente un nœud dans un document JSON. Elle peut être dérivée en `JsonObject`, `JsonArray`, ou `JsonValue`[1](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.nodes.jsonobject?view=net-9.0).
   - **Flexibilité** : `JsonNode` offre une flexibilité pour manipuler des structures JSON dynamiques sans connaître à l'avance leur type exact. Vous pouvez utiliser des méthodes comme `AsObject()`, `AsArray()`, et `AsValue()` pour convertir un `JsonNode` en son type dérivé spécifique[2](https://dev.to/vparab/working-with-jsonobject-jsonnode-jsonvalue-and-jsonarray-systemtextjson-api-5b8l).

2. **JsonObject** :
   - **Représentation d'un objet JSON** : `JsonObject` est une classe dérivée de `JsonNode` qui représente spécifiquement un objet JSON, c'est-à-dire une collection de paires clé-valeur[1](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.nodes.jsonobject?view=net-9.0).
   - **Interface de dictionnaire** : `JsonObject` implémente les interfaces `IDictionary<string, JsonNode>` et `ICollection<KeyValuePair<string, JsonNode>>`, ce qui permet de manipuler les propriétés de l'objet JSON comme un dictionnaire[1](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.nodes.jsonobject?view=net-9.0).
   - **Mutabilité** : `JsonObject` permet de modifier les propriétés de l'objet JSON après sa création, ce qui est utile pour construire ou mettre à jour des objets JSON de manière dynamique[3](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/use-dom).

Voici un exemple pour illustrer la différence :
```csharp
using System;
using System.Text.Json.Nodes;

public class Example
{
    public static void Main()
    {
        // Création d'un JsonObject
        var jsonObject = new JsonObject
        {
            ["name"] = "John Doe",
            ["age"] = 30,
            ["isEmployed"] = true
        };

        // Accès aux propriétés via JsonObject
        Console.WriteLine(jsonObject["name"]); // John Doe

        // Utilisation de JsonNode pour la flexibilité
        JsonNode jsonNode = jsonObject;
        if (jsonNode is JsonObject obj)
        {
            Console.WriteLine(obj["age"]); // 30
        }
    }
}
```

En résumé, `JsonObject` est une spécialisation de `JsonNode` pour les objets JSON, offrant des fonctionnalités spécifiques pour manipuler des paires clé-valeur, tandis que `JsonNode` fournit une base flexible pour travailler avec différents types de nœuds JSON.

Les scénarios d'utilisation typiques pour `JsonObject` et `JsonNode` en .NET incluent plusieurs situations où la manipulation de données JSON est nécessaire. Voici quelques exemples :

1. **Manipulation dynamique de JSON** :
   - **JsonNode** : Idéal pour les scénarios où la structure JSON est inconnue à l'avance ou peut changer. Par exemple, lors de la lecture de données JSON provenant d'une API externe où les propriétés peuvent varier.
   - **JsonObject** : Utilisé lorsque vous savez que vous travaillez avec un objet JSON et que vous souhaitez accéder ou modifier ses propriétés de manière structurée.

2. **Construction de documents JSON** :
   - **JsonObject** : Pratique pour construire des objets JSON de manière programmatique, en ajoutant des paires clé-valeur dynamiquement.
   ```csharp
   var jsonObject = new JsonObject
   {
       ["name"] = "John Doe",
       ["age"] = 30,
       ["isEmployed"] = true
   };
   ```

3. **Parsing et navigation dans des documents JSON** :
   - **JsonNode** : Permet de parcourir et de naviguer dans des documents JSON complexes, en accédant aux nœuds enfants de manière flexible.
   ```csharp
   JsonNode jsonNode = JsonNode.Parse(jsonString);
   string name = jsonNode["name"].GetValue<string>();
   ```

4. **Validation et transformation de données** :
   - **JsonObject** : Utilisé pour valider et transformer des objets JSON avant de les utiliser dans l'application. Par exemple, vérifier la présence de certaines propriétés ou transformer les valeurs.
   ```csharp
   if (jsonObject.ContainsKey("age"))
   {
       int age = jsonObject["age"].GetValue<int>();
       // Transformation ou validation de l'âge
   }
   ```

5. **Interopérabilité avec des systèmes externes** :
   - **JsonNode** et **JsonObject** : Utilisés pour interagir avec des services web, des bases de données NoSQL, ou d'autres systèmes qui utilisent JSON comme format d'échange de données.

6. **Sérialisation et désérialisation** :
   - **JsonObject** : Utilisé pour désérialiser des chaînes JSON en objets fortement typés, puis les manipuler et les resérialiser si nécessaire.
   ```csharp
   string jsonString = jsonObject.ToString();
   ```

Ces scénarios montrent comment `JsonObject` et `JsonNode` peuvent être utilisés pour manipuler des données JSON de manière flexible et efficace dans différentes situations.

## Sauvegarde du type de chaque élément dans un JsonObject
Oui, il est possible de sauvegarder le type de chaque élément dans un `JsonObject` en ajoutant des métadonnées sur les types. Une approche courante consiste à inclure des informations sur le type dans les propriétés de l'objet JSON. Voici un exemple de comment vous pouvez le faire :

1. **Ajouter des métadonnées de type** :
   Vous pouvez ajouter une propriété supplémentaire pour chaque élément qui indique son type.
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

2. **Sérialisation et désérialisation avec types** :
   Lors de la désérialisation, vous pouvez lire ces métadonnées pour déterminer le type de chaque propriété.
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
   Une autre approche consiste à créer une classe wrapper qui inclut à la fois la valeur et le type de chaque propriété.
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

Ces méthodes vous permettent de sauvegarder et de récupérer les types de chaque élément dans un `JsonObject`, ce qui peut être utile pour des scénarios où vous devez conserver des informations de type dynamiques.

## Sérialisation de la classe TypedProperty en JSON
La sérialisation de la classe `TypedProperty` en JSON implique la conversion d'une instance de cette classe en une chaîne JSON. Voici comment vous pouvez le faire en utilisant `System.Text.Json` :

1. **Définir la classe `TypedProperty`** :
   Cette classe contient deux propriétés : `Value` pour la valeur de la propriété et `Type` pour le type de la propriété.
   ```csharp
   public class TypedProperty
   {
       public object Value { get; set; }
       public string Type { get; set; }
   }
   ```

2. **Créer une instance de `TypedProperty`** :
   Vous pouvez créer des instances de cette classe pour différentes propriétés avec leurs valeurs et types respectifs.
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

3. **Sérialiser les instances en JSON** :
   Utilisez `JsonSerializer` pour convertir les instances de `TypedProperty` en chaînes JSON.
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

4. **Résultat de la sérialisation** :
   Les instances de `TypedProperty` seront sérialisées en JSON avec leurs valeurs et types respectifs.
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

5. **Désérialisation** :
   Pour désérialiser ces chaînes JSON en instances de `TypedProperty`, vous pouvez utiliser `JsonSerializer.Deserialize`.
   ```csharp
   var deserializedNameProperty = JsonSerializer.Deserialize<TypedProperty>(nameJson);
   var deserializedAgeProperty = JsonSerializer.Deserialize<TypedProperty>(ageJson);
   var deserializedIsEmployedProperty = JsonSerializer.Deserialize<TypedProperty>(isEmployedJson);

   Console.WriteLine($"Name: {deserializedNameProperty.Value}, Type: {deserializedNameProperty.Type}");
   Console.WriteLine($"Age: {deserializedAgeProperty.Value}, Type: {deserializedAgeProperty.Type}");
   Console.WriteLine($"Is Employed: {deserializedIsEmployedProperty.Value}, Type: {deserializedIsEmployedProperty.Type}");
   ```

Cette approche vous permet de sérialiser et désérialiser des propriétés avec leurs types, ce qui peut être utile pour des scénarios où vous devez conserver des informations de type dynamiques.

# Soluton de serialisation et désérialisation de données en passant le type de données dans le message

```csharp
# 

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

L'EventType de la structure en json doit correspondre à la structure choisi pour la partie propre à chaque businnes app (Admin, Reserving) : la partie serialiser en json Data.

## ApplicationDeltaEntity en json

### via le Publisher

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

### via le consumer

On stocke bien le bson dans la base de données MongoDB

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

## ApplicationTetaEntity en json

### via le Publisher
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

### Via le Consumer

On stocke bien le bson dans la base de données MongoDB. Les données sont correctement typées.

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