# Create serverless container

# create service account

    yc iam service-account create --name dev-ops

    yc iam service-account get dev-ops

# create serverless container

    yc serverless container create --name sense-tower-serverless-auth-dev

# create serverless container revision

    yc serverless container revision deploy --container-name sense-tower-serverless-auth-dev --image cr.yandex/crplei8g9hfd1jrmpva5/sc.st.auth.dev:2022-05-27.0 --cores 1 --core-fraction 20 --memory 2GB --concurrency 1 --execution-timeout 30s --service-account-id ajecumakadd0n6b2cbg0

