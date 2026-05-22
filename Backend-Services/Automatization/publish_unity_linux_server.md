# Temporary instruction to publish unity linux server

## Build Linux servers

Open Build => Build Specific => Build Linux Servers

## Copy build docker scripts to builded server scene

Create "build" folder inside builded scene and move all builded files to build forlder

From https://github.com/Sense-Capital/SenseTowerVR/tree/developers/Docker Docker folder in SenseTowerVR repository copy file to builded server scene on the level of build folder

## Edit build script adn run it

To smth like this:

    docker build -t cr.yandex/crplei8g9hfd1jrmpva5/sc.st.server.hall.dev:2022-05-20.0 .
    docker push cr.yandex/crplei8g9hfd1jrmpva5/sc.st.server.hall.dev:2022-05-20.0

