1) `./scripts/verify_env.sh`
2) `pnpm i && pnpm -C packages/core-agent test`
3) `./scripts/get_models.sh`
4) `cmake -S native -B native/build -G Ninja && cmake --build native/build`
5) Open Unity `apps/unity` and run `Scenes/Demo.unity`
6) `./scripts/build_android.sh` to produce an AAB.
