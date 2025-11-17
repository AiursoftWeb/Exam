# Exam - A sample project

[![MIT licensed](https://img.shields.io/badge/license-MIT-blue.svg)](https://gitlab.aiursoft.com/aiursoft/exam/-/blob/master/LICENSE)
[![Pipeline stat](https://gitlab.aiursoft.com/aiursoft/exam/badges/master/pipeline.svg)](https://gitlab.aiursoft.com/aiursoft/exam/-/pipelines)
[![Test Coverage](https://gitlab.aiursoft.com/aiursoft/exam/badges/master/coverage.svg)](https://gitlab.aiursoft.com/aiursoft/exam/-/pipelines)
[![ManHours](https://manhours.aiursoft.com/r/gitlab.aiursoft.com/aiursoft/exam.svg)](https://gitlab.aiursoft.com/aiursoft/exam/-/commits/master?ref_type=heads)
[![Website](https://img.shields.io/website?url=https%3A%2F%2Fexam.aiursoft.com)](https://exam.aiursoft.com)
[![Docker](https://img.shields.io/docker/pulls/aiursoft/exam.svg)](https://hub.docker.com/r/aiursoft/exam)

Exam is a sample project.

![screenshot](./screenshot.png)

Default user name is `admin@default.com` and default password is `admin123`.

## Projects using Aiursoft Exam

* [Stathub](https://gitlab.aiursoft.com/aiursoft/stathub)
* [MarkToHtml](https://gitlab.aiursoft.com/aiursoft/marktohtml)
* [MusicTools](https://gitlab.aiursoft.com/aiursoft/musictools)
* [AnduinOS Home](https://gitlab.aiursoft.com/anduin/AnduinOS-Home)

## Try

Try a running Exam [here](https://exam.aiursoft.com).

## Run in Ubuntu

The following script will install\update this app on your Ubuntu server. Supports Ubuntu 25.04.

On your Ubuntu server, run the following command:

```bash
curl -sL https://gitlab.aiursoft.com/aiursoft/exam/-/raw/master/install.sh | sudo bash
```

Of course it is suggested that append a custom port number to the command:

```bash
curl -sL https://gitlab.aiursoft.com/aiursoft/exam/-/raw/master/install.sh | sudo bash -s 8080
```

It will install the app as a systemd service, and start it automatically. Binary files will be located at `/opt/apps`. Service files will be located at `/etc/systemd/system`.

## Run manually

Requirements about how to run

1. Install [.NET 10 SDK](http://dot.net/) and [Node.js](https://nodejs.org/).
2. Execute `npm install` at `wwwroot` folder to install the dependencies.
3. Execute `dotnet run` to run the app.
4. Use your browser to view [http://localhost:5000](http://localhost:5000).

## Run in Microsoft Visual Studio

1. Open the `.sln` file in the project path.
2. Press `F5` to run the app.

## Run in Docker

First, install Docker [here](https://docs.docker.com/get-docker/).

Then run the following commands in a Linux shell:

```bash
image=aiursoft/exam
appName=exam
sudo docker pull $image
sudo docker run -d --name $appName --restart unless-stopped -p 5000:5000 -v /var/www/$appName:/data $image
```

That will start a web server at `http://localhost:5000` and you can test the app.

The docker image has the following context:

| Properties  | Value                           |
|-------------|---------------------------------|
| Image       | aiursoft/exam               |
| Ports       | 5000                            |
| Binary path | /app                            |
| Data path   | /data                           |
| Config path | /data/appsettings.json          |

## Database structure

```mermaid
erDiagram
    User ||--o{ ExamPaperSubmission : "Submits"
    ExamPaper ||--o{ ExamPaperSubmission : "Has"
    ExamPaper ||--o{ ExamPaperQuestion : "Contains"
    Question ||--o{ ExamPaperQuestion : "AppearsIn"

    ExamPaperSubmission ||--o{ ExamPaperQuestionAnswer : "Contains"
    ExamPaperQuestion ||--o{ ExamPaperQuestionAnswer : "Receives"

    Question }o..o{ ChoiceQuestion : "IsA"
    Question }o..o{ FillInBlankQuestion : "IsA"

    ChoiceQuestion ||--o{ Choice : "Has"

    User {
        string Id
        string DisplayName
        string AvatarRelativePath
    }
    ExamPaper {
        Guid Id
        string Title
        string Description
        TimeSpan Duration
        bool ShuffleQuestions
        bool AllowRetake
        int MaxRetakeCount
        int PassingScore
    }
    ExamPaperSubmission {
        Guid Id
        Guid ExamPaperId
        string UserId
        int TotalScore
    }
    Question {
        Guid Id
        string Discriminator
    }
    ExamPaperQuestion {
        Guid Id
        Guid ExamPaperId
        Guid QuestionId
        int Order
        int Score
    }
    ExamPaperQuestionAnswer {
        Guid Id
        Guid ExamPaperQuestionId
        Guid ExamPaperSubmissionId
        bool IsCorrect
        string AnswerContent
        int ObtainedScore
    }
    ChoiceQuestion {
        Guid Id
        string Content
    }
    FillInBlankQuestion {
        Guid Id
        string TextWithBlanks
        string Answer
    }
    Choice {
        Guid Id
        string Content
        bool IsCorrect
        Guid QuestionId
    }
```

## How to contribute

There are many ways to contribute to the project: logging bugs, submitting pull requests, reporting issues, and creating suggestions.

Even if you with push rights on the repository, you should create a personal fork and create feature branches there when you need them. This keeps the main repository clean and your workflow cruft out of sight.

We're also interested in your feedback on the future of this project. You can submit a suggestion or feature request through the issue tracker. To make this process more effective, we're asking that these include more information to help define them more clearly.
