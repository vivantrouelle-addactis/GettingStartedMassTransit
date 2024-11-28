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


# Passage de donner générique

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

3. **Envoyer un Message Générique** :
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

5. **Configurer le Bus et les Consommateurs** :
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