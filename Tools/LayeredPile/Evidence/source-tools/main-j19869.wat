  (func $j19869 (type 2) (param i32 i32) (result i32)
    (local i32 i32 i32 i32)
    global.get 0
    i32.const -64
    i32.add
    local.set 1
    local.get 1
    global.set 0
    i32.const 4509403
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3455724
      call $j870
      i32.const 3455732
      call $j870
      i32.const 3455736
      call $j870
      i32.const 3455712
      call $j870
      i32.const 3455748
      call $j870
      i32.const 3455752
      call $j870
      i32.const 3420448
      call $j870
      i32.const 3461832
      call $j870
      i32.const 3461836
      call $j870
      i32.const 3461840
      call $j870
      i32.const 3465140
      call $j870
      i32.const 3465144
      call $j870
      i32.const 3471620
      call $j870
      i32.const 3471608
      call $j870
      i32.const 3470264
      call $j870
      i32.const 3470268
      call $j870
      i32.const 3425124
      call $j870
      i32.const 4509403
      i32.const 1
      i32.store8
    end
    i32.const 0
    local.set 3
    local.get 1
    i32.const 0
    i32.store offset=56
    local.get 1
    local.get 3
    i32.store offset=60
    i32.const 0
    local.set 3
    local.get 1
    i32.const 0
    i32.store offset=48
    local.get 1
    local.get 3
    i32.store offset=52
    i32.const 0
    local.set 3
    local.get 1
    i32.const 0
    i32.store offset=40
    local.get 1
    local.get 3
    i32.store offset=44
    i32.const 0
    local.set 3
    local.get 1
    i32.const 0
    i32.store offset=32
    local.get 1
    local.get 3
    i32.store offset=36
    i32.const 3420448
    i32.load
    call $j873
    local.set 3
    local.get 3
    i32.const 3455712
    i32.load
    call $j920
    i32.const 3470264
    i32.load
    drop
    local.get 0
    i32.load offset=192
    i32.load offset=12
    local.set 2
    loop  ;; label = @1
      local.get 2
      i32.const 1
      i32.sub
      local.set 2
      local.get 2
      i32.const -1
      i32.gt_s
      if  ;; label = @2
        local.get 3
        local.get 0
        i32.load offset=192
        local.get 2
        i32.const 3470268
        i32.load
        call $j895
        i32.load offset=12
        i32.const 3455732
        i32.load
        call $j991
        local.set 5
        local.get 0
        i32.load offset=192
        local.get 2
        i32.const 3470268
        i32.load
        call $j895
        i32.load offset=12
        local.set 4
        local.get 5
        i32.eqz
        if  ;; label = @3
          local.get 3
          local.get 4
          i32.const 1
          i32.const 3455724
          i32.load
          call $j1136
          br 2 (;@1;)
        end
        local.get 3
        local.get 4
        local.get 3
        local.get 4
        i32.const 3455748
        i32.load
        call $j989
        i32.const 1
        i32.add
        i32.const 3455752
        i32.load
        call $j1871
        br 1 (;@1;)
      end
    end
    i32.const 3425124
    i32.load
    call $j873
    local.set 0
    local.get 0
    i32.const 3471608
    i32.load
    call $j918
    local.get 1
    i32.const 8
    i32.add
    local.get 3
    i32.const 3455736
    i32.load
    call $j1277
    local.get 1
    i32.load offset=24
    local.set 3
    local.get 1
    i32.load offset=28
    local.set 2
    local.get 1
    local.get 3
    i32.store offset=56
    local.get 1
    local.get 2
    i32.store offset=60
    local.get 1
    i32.load offset=16
    local.set 2
    local.get 1
    i32.load offset=20
    local.set 3
    local.get 1
    local.get 2
    i32.store offset=48
    local.get 1
    local.get 3
    i32.store offset=52
    local.get 1
    i32.load offset=8
    local.set 3
    local.get 1
    i32.load offset=12
    local.set 2
    local.get 1
    local.get 3
    i32.store offset=40
    local.get 1
    local.get 2
    i32.store offset=44
    local.get 1
    local.get 1
    i32.const 40
    i32.add
    i32.store
    local.get 1
    i32.const 0
    i32.store offset=8
    local.get 1
    local.get 1
    i32.load
    i32.store offset=12
    block  ;; label = @1
      block  ;; label = @2
        block  ;; label = @3
          block  ;; label = @4
            block  ;; label = @5
              loop  ;; label = @6
                i32.const 3986628
                i32.const 0
                i32.store
                i32.const 5876
                local.get 1
                i32.const 40
                i32.add
                i32.const 3461836
                i32.load
                call $fimport$2
                local.set 2
                i32.const 3986628
                i32.load
                local.set 3
                i32.const 3986628
                i32.const 0
                i32.store
                local.get 3
                i32.const 1
                i32.eq
                if  ;; label = @7
                  i32.const 2116560
                  call $fimport$0
                  local.set 2
                  br 4 (;@3;)
                end
                local.get 2
                i32.eqz
                br_if 4 (;@2;)
                i32.const 3986628
                i32.const 0
                i32.store
                i32.const 5877
                local.get 1
                local.get 1
                i32.const 40
                i32.add
                i32.const 3461840
                i32.load
                call $fimport$5
                i32.const 3986628
                i32.load
                local.set 2
                i32.const 3986628
                i32.const 0
                i32.store
                local.get 2
                i32.const 1
                i32.eq
                br_if 1 (;@5;)
                local.get 1
                i32.load
                local.set 2
                local.get 1
                i32.load offset=4
                local.set 3
                local.get 1
                local.get 2
                i32.store offset=32
                local.get 1
                local.get 3
                i32.store offset=36
                i32.const 3986628
                i32.const 0
                i32.store
                i32.const 5879
                local.get 1
                i32.const 32
                i32.add
                i32.const 3465144
                i32.load
                call $fimport$2
                local.set 2
                i32.const 3986628
                i32.load
                local.set 3
                i32.const 3986628
                i32.const 0
                i32.store
                local.get 3
                i32.const 1
                i32.eq
                br_if 2 (;@4;)
                local.get 2
                i32.const 3
                i32.lt_s
                br_if 0 (;@6;)
                i32.const 3986628
                i32.const 0
                i32.store
                i32.const 5878
                local.get 1
                i32.const 32
                i32.add
                i32.const 3465140
                i32.load
                call $fimport$2
                local.set 2
                i32.const 3986628
                i32.load
                local.set 3
                i32.const 3986628
                i32.const 0
                i32.store
                local.get 3
                i32.const 1
                i32.ne
                if  ;; label = @7
                  i32.const 3986628
                  i32.const 0
                  i32.store
                  i32.const 3047
                  local.get 0
                  local.get 2
                  i32.const 3471620
                  i32.load
                  call $fimport$5
                  i32.const 3986628
                  i32.load
                  local.set 2
                  i32.const 3986628
                  i32.const 0
                  i32.store
                  local.get 2
                  i32.const 1
                  i32.ne
                  br_if 1 (;@6;)
                end
              end
              i32.const 2116560
              call $fimport$0
              local.set 2
              br 2 (;@3;)
            end
            i32.const 2116560
            call $fimport$0
            local.set 2
            br 1 (;@3;)
          end
          i32.const 2116560
          call $fimport$0
          local.set 2
        end
        call $fimport$1
        i32.const 2116560
        call $fimport$9
        i32.eq
        if  ;; label = @3
          local.get 1
          local.get 2
          call $fimport$8
          i32.load
          i32.store offset=8
          i32.const 3986628
          i32.const 0
          i32.store
          i32.const 391
          call $fimport$6
          i32.const 3986628
          i32.load
          local.set 2
          i32.const 3986628
          i32.const 0
          i32.store
          local.get 2
          i32.const 1
          i32.ne
          br_if 1 (;@2;)
          call $fimport$4
          local.set 2
          call $fimport$1
          drop
        end
        i32.const 3986628
        i32.const 0
        i32.store
        i32.const 6090
        local.get 1
        i32.const 8
        i32.add
        call $fimport$3
        drop
        i32.const 3986628
        i32.load
        local.set 0
        i32.const 3986628
        i32.const 0
        i32.store
        local.get 0
        i32.const 1
        i32.ne
        br_if 1 (;@1;)
        i32.const 0
        call $fimport$0
        drop
        call $fimport$1
        drop
        call $j894
        unreachable
      end
      local.get 1
      i32.const 8
      i32.add
      call $j3434
      drop
      local.get 1
      i32.const -64
      i32.sub
      global.set 0
      local.get 0
      return
    end
    local.get 2
    call $fimport$10
    unreachable)
