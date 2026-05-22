# create MongoDB cluster and databases

## create cluster

    yc managed-mongodb cluster create --name mongodb_dev --mongodb-version 5.0 --environment=production --network-name sense-tower-network --host zone-id=ru-central1-a,subnet-name=sense-tower-subnet-a,assign-public-ip --mongod-resource-preset b2.nano --user name=Sc_St_Identity0,password=M0ng0DB_!dentity --database name=Identity --mongod-disk-type network-hdd --mongod-disk-size 10 --deletion-protection=true

## create additional database

    yc managed-mongodb database create Identity_dev --cluster-name mongodb_dev

## grant permissions to user

    yc managed-mongodb user update Sc_St_Identity0 --cluster-name mongodb_dev --permission database=admin,role=mdbShardingManager,role=mdbMonitor --permission database=Identity,role=mdbDbAdmin,role=readWrite --permission database=Identity_dev,role=mdbDbAdmin,role=readWrite
