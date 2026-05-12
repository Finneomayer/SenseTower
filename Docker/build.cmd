docker build -t tst .
# docker run -e SenseTowerSettings_IsServer="true" -e Port="7777" -p 7777:7777/udp -it tst
docker push tst