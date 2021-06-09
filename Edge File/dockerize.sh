#!/bin/bash

echo ""
echo ">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>"
echo ">>> Dockerizing POC3 Edge Server"
echo ">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>"
echo ""

# Dokcerize POC3 Edge Server
sudo docker build --no-cache -t meep-docker-registry:30001/server-poc-cloud .
sudo docker push meep-docker-registry:30001/server-poc-cloud

echo ""
echo ">>> POC3 Edge Server dockerize completed"
