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

## Cartoon Pop replacement (2026-11-02)

Default combine ID 23 now selects `unfa_cartoon_pop.wav`: Cartoon Pop (Clean) by unfa, CC0, https://freesound.org/people/unfa/sounds/245645/ . Source is the author page’s public high-quality MP3 preview, https://cdn.freesound.org/previews/245/245645_1038806-hq.mp3 (original FLAC requires login). Decoded to 16-bit PCM WAV, 48 kHz stereo, 8,254 frames (~0.172 seconds); converting does not restore lossless source quality. No gain, EQ, pitch or timing changes. Unity import uses PCM, original sample rate, no normalization or mono conversion. Previous pop assets retained.

Validation: WAV decoded and catalog ID 23 verified; git diff check passed. Live Unity editor did not complete the temporary import validation; editor playback and device playback remain unverified.


## UI button click replacement (2026-11-03)

UI button clicks now use Zippo click sound by dawith (submitted by qubodup), CC0: https://opengameart.org/content/zippo-click-sound . Downloaded original https://opengameart.org/sites/default/files/zipclick.flac and losslessly decoded to `Assets/GooseReplica/Resources/GooseArt/Audio/S_ButtonClick.wav` (16-bit PCM, 44,100 Hz stereo, 22,049 frames, ~0.500 seconds). No gain, EQ, trim or pitch changes. Preserved the asset GUID so ThemeLibrary.clickSound and ReplicaBuilder keep resolving to the replacement. Unity import uses PCM with normalization disabled and preloading enabled. Item selection and combine audio are unchanged.

Validation: source/output decoded PCM comparison, WAV format and existing button reference verified. In-editor/device playback not tested; existing WeChat builds need rebuilding to include the replacement.


## Shared success / failure sounds (2026-11-03)

Replaced `GooseArt/Audio/S_Win.wav` and `S_Lose.wav` with decoded copies of `/Users/liuzhenhua/Desktop/code/crazyCombo/client/assets/resources/sounds/Win.mp3` and `Lose.mp3`, as requested. Confirmed the source main and result scenes reference these clips for victory and failure. Decoded to 16-bit PCM WAV at original sample rate/channels without gain, EQ or trimming; this does not restore the MP3's lost information. Unity import uses PCM, normalization disabled and preloading enabled. Asset GUIDs preserved, so ThemeLibrary and ReplicaBuilder retain their existing references. Result playback timing, volume and effects toggle are unchanged.

Validation: both output clips decode successfully and their PCM SHA-256 hashes match the source MP3s decoded to the same format; ThemeLibrary GUID references verified; JSON parses and git diff check passed. In-editor/device playback remains untested. Existing WeChat builds require rebuilding to include these assets.
