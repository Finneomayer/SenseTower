#!/bin/bash

INPUT_BUILD_DIR="./Builds"
BUILD_VERSION="00000000"
DOCKER_REGISTRY="${DOCKER_REGISTRY:-}"
# DOCKER_IMAGE="linux-server"
DOCKER_IMAGE="sc.st.server"

if [ ! -z "${1}" ]; then
    INPUT_BUILD_DIR="${1}"
fi

if [ ! -z "${2}" ]; then
    BUILD_VERSION="${2}"
fi

if [ ! -z "${DOCKER_REGISTRY}" ]; then
    DOCKER_IMAGE="${DOCKER_REGISTRY}/${DOCKER_IMAGE}"
fi

# build base image
pushd ./Docker
docker build --tag linux-server-base -f Dockerfile.base .
popd

# build server variants
for buildedScene in $(find "${INPUT_BUILD_DIR}" -mindepth 1 -maxdepth 1 -type d); do
    # copy scripts for build time optimisation (because docker copy ALL builed scenes on context)
    cp -f ./Docker/Dockerfile ./Docker/entrypoint.sh "${buildedScene}/"
    pushd "${buildedScene}"
    suffix=".unity"
    scene="$(basename ${buildedScene,,})"
    dockerTag="${DOCKER_IMAGE}.${scene%${suffix}}.dev:${BUILD_VERSION}"
    docker build --file ./Dockerfile \
        --label io.sensetower.build_variant=server \
        --label io.sensetower.version="${BUILD_VERSION}" \
        --tag "${dockerTag}" \
        .
        # "${DOCKER_IMAGE}-$(basename ${buildedScene,,}):${BUILD_VERSION}"
    if [ ! -z "${DOCKER_REGISTRY}" ]; then
      docker push "${dockerTag}"
      docker rmi "${dockerTag}"
    fi
    popd
done
