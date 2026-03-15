
## Audio Library abstractions.

This directory contains abstractions for manipulating audio data - these definitions provided here are used by the Music Player app to construct the base of how tracks should be played back.

This is a complete re-design of the NAudio project in a try to modernize the interface and make it cross-platform, while keeping the classical byte arrays around.

It additionally provides utilities that can be used in all platforms such as the `AudioRenderingBuffer`,
and the `MonoToStereoAudioProvider` classes.
