FROM ubuntu:20.04

LABEL maintainer="john.cartmell@interdigital.com"

RUN apt-get update && apt-get install -y \
    x11vnc xvfb libglu1-mesa-dev mesa-utils xterm vim net-tools tcpdump && \
    mkdir ~/.vnc && \
    touch ~/.vnc/passwd

RUN x11vnc -storepasswd "devops" ~/.vnc/passwd

EXPOSE 7777 

COPY . /home/ubuntu/

CMD sh /home/ubuntu/commands.sh
