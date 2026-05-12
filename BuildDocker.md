Copy files from Docker directory to the directory up to your Linux build results, and rename results folder to build  and execute build.cmd

```
docker build -t tst3 .
docker run -e SenseTowerSettings_IsServer="true" -e Port="7777" -p 7777:7777/udp -it tst3
```
