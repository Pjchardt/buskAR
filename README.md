![alt text](https://pjchardt.github.io/images/buskAR.jpg)
# buskAR
Music performance app for busker with a networked augmented reality visualization for viewers. Uses <a href="https://unity3d.com/">Unity</a>, <a href="https://www.photonengine.com/en/PUN">Photon Networking</a>, and <a href="https://developers.google.com/ar/discover/">ARCore</a>. Buskar plays a sequencer, viewers connect over the network and see an AR visualization of the music in space. 

## Project
buskAR is part of my ongoing project around AR. The main website is at <a href="https://oeoe.club">oeoe.club</a>.

buskAR is one exploration into networked audio/visual experiences and performance using AR. The core idea involves networking to sync data between devices, then allowing the sync to drive experiences congtrolled by one person or a group of people. The project is open source under the MIT license so folks interested in networked AR have a starting point. 

Let me know if you have any questions or feedback!

## Getting Started

1. Download or pull the project.
2. Open Unity (last built with 2017.1.f3)
3. Put in your own <a href="https://doc.photonengine.com/en-us/realtime/current/getting-started/obtain-your-app-id">Photon App ID</a>
4. Build out to android phone that supports ARCore
5. Run Master from pc or phone and create a room name, then connect.
6. Connect phone as viewer with same room name.
7. Play around on master and view visualization on viewer(s) after placing visualization root.

## Moving Forward

- Add more visualization effects
- Add more interesting interactrion on viewer
