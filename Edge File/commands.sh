# Start the first instance of the edge on virtual display 1.
# Xvfb :1 -screen 0 512x368x16 -ac &
# /usr/bin/x11vnc -display :1.0 -shared -forever -usepw -autoport 32000 &
# sleep 5 
/home/ubuntu/game_server.x86_64 -s
