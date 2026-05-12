FROM linux-server-base

ARG ARTIFACTS_SOURCE=build/

COPY entrypoint.sh           /entrypoint.sh
RUN  chmod +x                /entrypoint.sh

COPY ${ARTIFACTS_SOURCE}        /root/build/
RUN  chmod +x                /root/build/linux_build.x86_64

WORKDIR /root/
ENTRYPOINT ["/bin/bash", "/entrypoint.sh"]

EXPOSE 7777/udp