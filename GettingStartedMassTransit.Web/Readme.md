# Reference
https://github.com/MassTransit/Sample-GettingStarted

https://masstransit.io/quick-starts/rabbitmq

https://masstransit.io/quick-starts/in-memory

# Docker RabbitMQ
locahost:15672

login : guest
Mode de passe : guest

# AWS
Creation d'un user au niveau de la sandbox AWS

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

