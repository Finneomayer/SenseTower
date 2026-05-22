# Publish docker to yandex cloud

## Step 0. Install and configure yandex CLI

https://cloud.yandex.ru/docs/cli/quickstart

In Windows:

    iex (New-Object System.Net.WebClient).DownloadString('https://storage.yandexcloud.net/yandexcloud-yc/install.ps1')

In Linux:

   curl -sSL https://storage.yandexcloud.net/yandexcloud-yc/install.sh | bash

## Step 1. Create container registry

Create registry in console:

    yc container registry create sense-tower-registry-dev

Configure your local docker to this registry:

    yc container registry configure-docker

## Step 2. Build your docker

    docker build -t cr.yandex/crp3nurift6e1qs053vk/sc.st.auth.dev:2022-04-27.0 .
    
Where"@crp3nurift6e1qs053vk" is your container registry id and "sc.st.auth.dev" is your image name and "2022-04-27.0" is your image version.

## Step 3. Publish your docker

    docker push cr.yandex/crp3nurift6e1qs053vk/sc.st.auth.dev:2022-04-27.0
