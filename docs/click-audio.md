# Item selection audio

DM-CGS-32.wav from Dustyroom Casual Game Sounds (CC0).
Source: https://dustyroom.com/free-casual-game-sounds/
User-selected original: /Users/liuzhenhua/Downloads/DM-CGS/WAV/DM-CGS-32.wav

The WAV is copied byte-for-byte: 44,100 Hz, stereo, 24-bit PCM, 8,253 frames (0.187143 seconds). Unity import uses PCM, original sample rate, no mono conversion or normalization. No lossy MP3 transcode is performed.

Only item selection playback uses this clip. Original sound IDs still control the 50 ms gate; usePitch and consecutive object-name sequencing retain 1 / 1.15 / 1.3 pitches. Combine, tools and additional goose voices keep their existing routing. The clip is preloaded with item audio; original selection clips remain as fallback.

Validation: Unity 2022.3 imported and decoded the selected WAV (8,253 frames, 44,100 Hz, stereo, PCM). Existing SourceAudioValidation passed for 83 profiles, pitch sequence, reset behavior and 50 ms gate. Editor validation only; WeChat/device playback has not been tested.

## Match / combine audio

Default combine sound ID 23 now resolves to `pop3.ogg`, from wubitog's CC0 3 Pop Sounds: https://opengameart.org/content/3-pop-sounds . User-selected source: /Users/liuzhenhua/Downloads/3pops/pop3.ogg . Copied byte-for-byte; 44,100 Hz stereo Vorbis, approximately 0.317 seconds. Unity import uses PCM to avoid another lossy encoding pass. Match completion timing, volume, pitch, clicking and special-effect routes are unchanged.

Combine validation: Unity imported and decoded pop3, ID 23 resolved to pop3, and selection still resolved to DM-CGS-32. Device playback not tested.

## Brighter combine variant (2026-10-29)

Default combine ID 23 now selects pop3_bright.wav. Derived from the preserved pop3.ogg with FFmpeg `treble=g=3:f=1800,volume=6dB`: gentle +3 dB high shelf at 1.8 kHz, plus +6 dB gain. Saved as 24-bit PCM WAV to avoid another lossy source encode. Same 13,985 stereo frames at 44,100 Hz. Sample peak -5.666 dBFS, no clipped samples. Clicking, pitch sequencing and event timing unchanged. WeChat needs a new build/upload to receive this change.
