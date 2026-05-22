FROM unityci/editor:ubuntu-2021.2.6f1-android-1.0

ARG DEBIAN_FRONTEND=noninteractive
ARG DOCKER_VERSION=17.06.0-ce

RUN apt-get update && apt-get install -y \
ca-certificates  \
libglu1 xvfb libxcursor1

COPY build/                  /root/build/
COPY entrypoint.sh           /entrypoint.sh

WORKDIR /root/
ENTRYPOINT ["/bin/bash", "/entrypoint.sh"]

EXPOSE 7777/udp