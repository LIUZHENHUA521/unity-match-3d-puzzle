  (func $j19949 (type 1) (param i32 i32 i32)
    (local i32 i32 i32 i32 i32 i32 i32 i32 i32 i32 i32 i32)
    global.get 0
    i32.const 16
    i32.sub
    local.set 6
    local.get 6
    global.set 0
    local.get 6
    local.get 1
    i32.store offset=12
    i32.const 4511503
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3471288
      call $j870
      i32.const 4511503
      i32.const 1
      i32.store8
    end
    local.get 6
    local.set 10
    local.get 2
    i32.load offset=16
    i32.load offset=96
    i32.const 2
    call $j874
    local.set 1
    local.get 1
    i32.load offset=20
    i32.const -1
    i32.le_s
    if  ;; label = @1
      local.get 1
      i32.load offset=128
      i32.const 8
      i32.sub
      local.set 1
    else
      i32.const 4
      local.set 1
    end
    local.get 1
    local.set 11
    local.get 11
    i32.const 15
    i32.add
    i32.const -16
    i32.and
    local.set 1
    local.get 10
    local.get 1
    i32.sub
    local.set 8
    local.get 8
    global.set 0
    local.get 8
    local.get 1
    i32.sub
    local.set 10
    local.get 10
    local.set 3
    local.get 3
    global.set 0
    local.get 3
    local.get 1
    i32.sub
    local.set 13
    local.get 13
    local.set 3
    local.get 3
    global.set 0
    local.get 3
    local.get 1
    i32.sub
    local.set 1
    local.get 1
    global.set 0
    local.get 1
    i32.const 0
    local.get 11
    call $j890
    local.set 14
    local.get 6
    i32.const 0
    i32.store offset=4
    local.get 0
    i32.load offset=16
    local.set 1
    local.get 2
    i32.load offset=16
    i32.load offset=96
    i32.const 2
    call $j874
    i32.load offset=20
    i32.const 31
    i32.shr_u
    local.set 3
    local.get 8
    local.get 6
    i32.load offset=12
    local.get 6
    i32.const 12
    i32.add
    local.get 3
    select
    local.get 11
    call $j885
    drop
    block  ;; label = @1
      local.get 2
      i32.load offset=16
      i32.load offset=96
      i32.load offset=12
      i32.load
      local.set 9
      local.get 2
      i32.load offset=16
      i32.load offset=96
      i32.load offset=12
      local.set 12
      local.get 2
      i32.load offset=16
      i32.load offset=96
      i32.const 2
      call $j874
      i32.load offset=20
      i32.const 31
      i32.shr_u
      if  ;; label = @2
        local.get 8
        local.set 3
      else
        local.get 8
        i32.load
        local.set 3
      end
      local.get 9
      local.get 12
      local.get 1
      local.get 3
      call $j1102
      i32.eqz
      br_if 0 (;@1;)
      local.get 0
      i32.load offset=16
      local.set 1
      local.get 2
      local.set 3
      local.get 2
      i32.load offset=16
      i32.load offset=96
      local.set 2
      i32.const 3986628
      i32.const 0
      i32.store
      i32.const 3205
      local.get 2
      i32.const 2
      call $fimport$2
      local.set 2
      i32.const 3986628
      i32.load
      local.set 4
      i32.const 3986628
      i32.const 0
      i32.store
      block  ;; label = @2
        block  ;; label = @3
          local.get 4
          i32.const 1
          i32.eq
          br_if 0 (;@3;)
          i32.const 3986628
          i32.const 0
          i32.store
          local.get 2
          i32.load offset=20
          i32.const 31
          i32.shr_u
          local.set 2
          i32.const 3986628
          i32.load
          local.set 4
          i32.const 3986628
          i32.const 0
          i32.store
          local.get 4
          i32.const 1
          i32.eq
          br_if 0 (;@3;)
          local.get 8
          local.get 6
          i32.load offset=12
          local.get 6
          i32.const 12
          i32.add
          local.get 2
          select
          local.get 11
          call $j885
          drop
          local.get 3
          i32.load offset=16
          i32.load offset=96
          i32.load offset=48
          i32.load
          local.set 4
          local.get 3
          i32.load offset=16
          i32.load offset=96
          i32.load offset=48
          local.set 5
          local.get 3
          i32.load offset=16
          i32.load offset=96
          local.set 2
          i32.const 3986628
          i32.const 0
          i32.store
          i32.const 3205
          local.get 2
          i32.const 2
          call $fimport$2
          local.set 2
          i32.const 3986628
          i32.load
          local.set 7
          i32.const 3986628
          i32.const 0
          i32.store
          block  ;; label = @4
            local.get 7
            i32.const 1
            i32.eq
            br_if 0 (;@4;)
            i32.const 3986628
            i32.const 0
            i32.store
            local.get 2
            i32.load offset=20
            i32.const 31
            i32.shr_u
            local.set 7
            i32.const 3986628
            i32.load
            local.set 2
            i32.const 3986628
            i32.const 0
            i32.store
            local.get 2
            i32.const 1
            i32.eq
            br_if 0 (;@4;)
            local.get 8
            local.set 2
            local.get 7
            i32.eqz
            if  ;; label = @5
              local.get 8
              i32.load
              local.set 2
            end
            i32.const 3986628
            i32.const 0
            i32.store
            i32.const 6705
            local.get 4
            local.get 5
            local.get 1
            local.get 2
            call $fimport$16
            local.set 1
            i32.const 3986628
            i32.load
            local.set 2
            i32.const 3986628
            i32.const 0
            i32.store
            local.get 2
            i32.const 1
            i32.eq
            br_if 0 (;@4;)
            local.get 0
            local.get 1
            i32.store offset=32
            local.get 0
            i32.load offset=8
            local.set 2
            local.get 3
            i32.load offset=16
            i32.load offset=96
            i32.load offset=52
            i32.load
            local.set 4
            local.get 3
            i32.load offset=16
            i32.load offset=96
            i32.load offset=52
            local.set 5
            i32.const 3986628
            i32.const 0
            i32.store
            i32.const 1343
            local.get 4
            local.get 5
            local.get 2
            local.get 1
            call $fimport$13
            i32.const 3986628
            i32.load
            local.set 1
            i32.const 3986628
            i32.const 0
            i32.store
            local.get 1
            i32.const 1
            i32.ne
            if  ;; label = @5
              local.get 0
              i32.load offset=32
              local.set 1
              local.get 0
              i32.load offset=12
              local.set 2
              i32.const 3986628
              i32.const 0
              i32.store
              i32.const 6730
              local.get 2
              local.get 1
              i32.const 3471288
              i32.load
              call $fimport$5
              i32.const 3986628
              i32.load
              local.set 1
              i32.const 3986628
              i32.const 0
              i32.store
              local.get 1
              i32.const 1
              i32.ne
              if  ;; label = @6
                local.get 0
                i32.load offset=16
                local.set 1
                local.get 3
                i32.load offset=16
                i32.load offset=96
                local.set 2
                i32.const 3986628
                i32.const 0
                i32.store
                i32.const 3205
                local.get 2
                i32.const 2
                call $fimport$2
                local.set 2
                i32.const 3986628
                i32.load
                local.set 4
                i32.const 3986628
                i32.const 0
                i32.store
                block  ;; label = @7
                  local.get 4
                  i32.const 1
                  i32.eq
                  br_if 0 (;@7;)
                  i32.const 3986628
                  i32.const 0
                  i32.store
                  local.get 2
                  i32.load offset=20
                  i32.const 31
                  i32.shr_u
                  local.set 2
                  i32.const 3986628
                  i32.load
                  local.set 4
                  i32.const 3986628
                  i32.const 0
                  i32.store
                  local.get 4
                  i32.const 1
                  i32.eq
                  br_if 0 (;@7;)
                  local.get 10
                  local.get 6
                  i32.load offset=12
                  local.get 6
                  i32.const 12
                  i32.add
                  local.get 2
                  select
                  local.get 11
                  call $j885
                  drop
                  local.get 3
                  i32.load offset=16
                  i32.load offset=96
                  i32.load offset=56
                  i32.load
                  local.set 4
                  local.get 3
                  i32.load offset=16
                  i32.load offset=96
                  i32.load offset=56
                  local.set 5
                  local.get 3
                  i32.load offset=16
                  i32.load offset=96
                  local.set 2
                  i32.const 3986628
                  i32.const 0
                  i32.store
                  i32.const 3205
                  local.get 2
                  i32.const 2
                  call $fimport$2
                  local.set 2
                  i32.const 3986628
                  i32.load
                  local.set 7
                  i32.const 3986628
                  i32.const 0
                  i32.store
                  block  ;; label = @8
                    local.get 7
                    i32.const 1
                    i32.eq
                    br_if 0 (;@8;)
                    i32.const 3986628
                    i32.const 0
                    i32.store
                    local.get 2
                    i32.load offset=20
                    i32.const 31
                    i32.shr_u
                    local.set 7
                    i32.const 3986628
                    i32.load
                    local.set 2
                    i32.const 3986628
                    i32.const 0
                    i32.store
                    local.get 2
                    i32.const 1
                    i32.eq
                    br_if 0 (;@8;)
                    local.get 10
                    local.set 2
                    local.get 7
                    i32.eqz
                    if  ;; label = @9
                      local.get 10
                      i32.load
                      local.set 2
                    end
                    i32.const 3986628
                    i32.const 0
                    i32.store
                    i32.const 6099
                    local.get 4
                    local.get 5
                    local.get 1
                    local.get 2
                    call $fimport$16
                    drop
                    i32.const 3986628
                    i32.load
                    local.set 1
                    i32.const 3986628
                    i32.const 0
                    i32.store
                    local.get 1
                    i32.const 1
                    i32.eq
                    br_if 0 (;@8;)
                    local.get 0
                    i32.load offset=16
                    local.set 1
                    local.get 3
                    i32.load offset=16
                    i32.load offset=96
                    i32.load offset=60
                    i32.load
                    local.set 2
                    local.get 3
                    i32.load offset=16
                    i32.load offset=96
                    i32.load offset=60
                    local.set 4
                    i32.const 3986628
                    i32.const 0
                    i32.store
                    i32.const 6731
                    local.get 2
                    local.get 4
                    local.get 1
                    call $fimport$7
                    local.set 2
                    i32.const 3986628
                    i32.load
                    local.set 1
                    i32.const 3986628
                    i32.const 0
                    i32.store
                    local.get 1
                    i32.const 1
                    i32.ne
                    if  ;; label = @9
                      local.get 3
                      i32.load offset=16
                      i32.load offset=96
                      local.set 1
                      i32.const 3986628
                      i32.const 0
                      i32.store
                      i32.const 3205
                      local.get 1
                      i32.const 17
                      call $fimport$2
                      local.set 1
                      i32.const 3986628
                      i32.load
                      local.set 4
                      i32.const 3986628
                      i32.const 0
                      i32.store
                      block  ;; label = @10
                        local.get 4
                        i32.const 1
                        i32.eq
                        br_if 0 (;@10;)
                        i32.const 3986628
                        i32.const 0
                        i32.store
                        i32.const 1159
                        local.get 1
                        call $fimport$3
                        local.set 1
                        i32.const 3986628
                        i32.load
                        local.set 4
                        i32.const 3986628
                        i32.const 0
                        i32.store
                        local.get 4
                        i32.const 1
                        i32.eq
                        br_if 0 (;@10;)
                        local.get 3
                        i32.load offset=16
                        i32.load offset=96
                        i32.load offset=72
                        i32.load
                        local.set 4
                        local.get 3
                        i32.load offset=16
                        i32.load offset=96
                        i32.load offset=72
                        local.set 5
                        i32.const 3986628
                        i32.const 0
                        i32.store
                        i32.const 3311
                        local.get 4
                        local.get 5
                        local.get 1
                        local.get 2
                        call $fimport$13
                        i32.const 3986628
                        i32.load
                        local.set 2
                        i32.const 3986628
                        i32.const 0
                        i32.store
                        local.get 2
                        i32.const 1
                        i32.eq
                        br_if 0 (;@10;)
                        local.get 0
                        local.get 1
                        i32.store offset=36
                        i32.const 0
                        local.set 2
                        block  ;; label = @11
                          block  ;; label = @12
                            block  ;; label = @13
                              block  ;; label = @14
                                block  ;; label = @15
                                  loop  ;; label = @16
                                    local.get 0
                                    i32.load offset=36
                                    local.set 1
                                    local.get 3
                                    i32.load offset=16
                                    i32.load offset=96
                                    i32.load offset=88
                                    i32.load
                                    local.set 4
                                    local.get 3
                                    i32.load offset=16
                                    i32.load offset=96
                                    i32.load offset=88
                                    local.set 5
                                    i32.const 3986628
                                    i32.const 0
                                    i32.store
                                    i32.const 3223
                                    local.get 4
                                    local.get 5
                                    local.get 1
                                    call $fimport$7
                                    local.set 1
                                    i32.const 3986628
                                    i32.load
                                    local.set 4
                                    i32.const 3986628
                                    i32.const 0
                                    i32.store
                                    local.get 4
                                    i32.const 1
                                    i32.eq
                                    br_if 5 (;@11;)
                                    local.get 1
                                    local.get 2
                                    i32.le_s
                                    br_if 15 (;@1;)
                                    local.get 0
                                    i32.load offset=16
                                    local.set 4
                                    local.get 0
                                    i32.load offset=36
                                    local.set 1
                                    local.get 3
                                    i32.load offset=16
                                    i32.load offset=96
                                    i32.load offset=80
                                    i32.load
                                    local.set 5
                                    local.get 3
                                    i32.load offset=16
                                    i32.load offset=96
                                    i32.load offset=80
                                    local.set 7
                                    i32.const 3986628
                                    i32.const 0
                                    i32.store
                                    i32.const 6732
                                    local.get 5
                                    local.get 7
                                    local.get 1
                                    local.get 2
                                    local.get 8
                                    call $fimport$17
                                    i32.const 3986628
                                    i32.load
                                    local.set 1
                                    i32.const 3986628
                                    i32.const 0
                                    i32.store
                                    local.get 1
                                    i32.const 1
                                    i32.ne
                                    if  ;; label = @17
                                      local.get 3
                                      i32.load offset=16
                                      i32.load offset=96
                                      i32.load offset=48
                                      i32.load
                                      local.set 5
                                      local.get 3
                                      i32.load offset=16
                                      i32.load offset=96
                                      i32.load offset=48
                                      local.set 7
                                      local.get 3
                                      i32.load offset=16
                                      i32.load offset=96
                                      local.set 1
                                      i32.const 3986628
                                      i32.const 0
                                      i32.store
                                      i32.const 3205
                                      local.get 1
                                      i32.const 2
                                      call $fimport$2
                                      local.set 1
                                      i32.const 3986628
                                      i32.load
                                      local.set 9
                                      i32.const 3986628
                                      i32.const 0
                                      i32.store
                                      local.get 9
                                      i32.const 1
                                      i32.eq
                                      br_if 2 (;@15;)
                                      i32.const 3986628
                                      i32.const 0
                                      i32.store
                                      local.get 1
                                      i32.load offset=20
                                      i32.const 31
                                      i32.shr_u
                                      local.set 9
                                      i32.const 3986628
                                      i32.load
                                      local.set 1
                                      i32.const 3986628
                                      i32.const 0
                                      i32.store
                                      local.get 1
                                      i32.const 1
                                      i32.eq
                                      br_if 2 (;@15;)
                                      local.get 8
                                      local.set 1
                                      local.get 9
                                      i32.eqz
                                      if  ;; label = @18
                                        local.get 8
                                        i32.load
                                        local.set 1
                                      end
                                      i32.const 3986628
                                      i32.const 0
                                      i32.store
                                      i32.const 6705
                                      local.get 5
                                      local.get 7
                                      local.get 4
                                      local.get 1
                                      call $fimport$16
                                      local.set 1
                                      i32.const 3986628
                                      i32.load
                                      local.set 4
                                      i32.const 3986628
                                      i32.const 0
                                      i32.store
                                      local.get 4
                                      i32.const 1
                                      i32.eq
                                      br_if 2 (;@15;)
                                      local.get 0
                                      i32.load offset=32
                                      local.get 1
                                      i32.lt_s
                                      if  ;; label = @18
                                        local.get 0
                                        i32.load offset=16
                                        local.set 4
                                        local.get 0
                                        i32.load offset=36
                                        local.set 1
                                        local.get 3
                                        i32.load offset=16
                                        i32.load offset=96
                                        i32.load offset=80
                                        i32.load
                                        local.set 5
                                        local.get 3
                                        i32.load offset=16
                                        i32.load offset=96
                                        i32.load offset=80
                                        local.set 7
                                        i32.const 3986628
                                        i32.const 0
                                        i32.store
                                        i32.const 6732
                                        local.get 5
                                        local.get 7
                                        local.get 1
                                        local.get 2
                                        local.get 8
                                        call $fimport$17
                                        i32.const 3986628
                                        i32.load
                                        local.set 1
                                        i32.const 3986628
                                        i32.const 0
                                        i32.store
                                        local.get 1
                                        i32.const 1
                                        i32.eq
                                        br_if 4 (;@14;)
                                        local.get 14
                                        local.get 8
                                        local.get 11
                                        call $j885
                                        drop
                                        local.get 10
                                        local.get 14
                                        local.get 11
                                        call $j885
                                        drop
                                        local.get 3
                                        i32.load offset=16
                                        i32.load offset=96
                                        i32.load offset=48
                                        i32.load
                                        local.set 5
                                        local.get 3
                                        i32.load offset=16
                                        i32.load offset=96
                                        i32.load offset=48
                                        local.set 7
                                        local.get 3
                                        i32.load offset=16
                                        i32.load offset=96
                                        local.set 1
                                        i32.const 3986628
                                        i32.const 0
                                        i32.store
                                        i32.const 3205
                                        local.get 1
                                        i32.const 2
                                        call $fimport$2
                                        local.set 1
                                        i32.const 3986628
                                        i32.load
                                        local.set 9
                                        i32.const 3986628
                                        i32.const 0
                                        i32.store
                                        local.get 9
                                        i32.const 1
                                        i32.eq
                                        br_if 5 (;@13;)
                                        i32.const 3986628
                                        i32.const 0
                                        i32.store
                                        local.get 1
                                        i32.load offset=20
                                        i32.const 31
                                        i32.shr_u
                                        local.set 9
                                        i32.const 3986628
                                        i32.load
                                        local.set 1
                                        i32.const 3986628
                                        i32.const 0
                                        i32.store
                                        local.get 1
                                        i32.const 1
                                        i32.eq
                                        br_if 5 (;@13;)
                                        local.get 10
                                        local.set 1
                                        local.get 9
                                        i32.eqz
                                        if  ;; label = @19
                                          local.get 10
                                          i32.load
                                          local.set 1
                                        end
                                        i32.const 3986628
                                        i32.const 0
                                        i32.store
                                        i32.const 6705
                                        local.get 5
                                        local.get 7
                                        local.get 4
                                        local.get 1
                                        call $fimport$16
                                        local.set 5
                                        i32.const 3986628
                                        i32.load
                                        local.set 1
                                        i32.const 3986628
                                        i32.const 0
                                        i32.store
                                        local.get 1
                                        i32.const 1
                                        i32.eq
                                        br_if 5 (;@13;)
                                        local.get 13
                                        local.get 14
                                        local.get 11
                                        call $j885
                                        drop
                                        local.get 3
                                        i32.load offset=16
                                        i32.load offset=96
                                        i32.load offset=84
                                        i32.load
                                        local.set 7
                                        local.get 3
                                        i32.load offset=16
                                        i32.load offset=96
                                        i32.load offset=84
                                        local.set 9
                                        local.get 3
                                        i32.load offset=16
                                        i32.load offset=96
                                        local.set 1
                                        i32.const 3986628
                                        i32.const 0
                                        i32.store
                                        i32.const 3205
                                        local.get 1
                                        i32.const 2
                                        call $fimport$2
                                        local.set 1
                                        i32.const 3986628
                                        i32.load
                                        local.set 12
                                        i32.const 3986628
                                        i32.const 0
                                        i32.store
                                        local.get 12
                                        i32.const 1
                                        i32.eq
                                        br_if 6 (;@12;)
                                        i32.const 3986628
                                        i32.const 0
                                        i32.store
                                        local.get 1
                                        i32.load offset=20
                                        i32.const 31
                                        i32.shr_u
                                        local.set 12
                                        i32.const 3986628
                                        i32.load
                                        local.set 1
                                        i32.const 3986628
                                        i32.const 0
                                        i32.store
                                        local.get 1
                                        i32.const 1
                                        i32.eq
                                        br_if 6 (;@12;)
                                        local.get 13
                                        local.set 1
                                        local.get 12
                                        i32.eqz
                                        if  ;; label = @19
                                          local.get 13
                                          i32.load
                                          local.set 1
                                        end
                                        i32.const 3986628
                                        i32.const 0
                                        i32.store
                                        i32.const 6733
                                        local.get 7
                                        local.get 9
                                        local.get 4
                                        local.get 1
                                        local.get 5
                                        i32.const 1
                                        i32.sub
                                        call $fimport$17
                                        i32.const 3986628
                                        i32.load
                                        local.set 1
                                        i32.const 3986628
                                        i32.const 0
                                        i32.store
                                        local.get 1
                                        i32.const 1
                                        i32.eq
                                        br_if 6 (;@12;)
                                      end
                                      local.get 2
                                      i32.const 1
                                      i32.add
                                      local.set 2
                                      br 1 (;@16;)
                                    end
                                  end
                                  i32.const 2116560
                                  call $fimport$0
                                  local.set 2
                                  br 13 (;@2;)
                                end
                                i32.const 2116560
                                call $fimport$0
                                local.set 2
                                br 12 (;@2;)
                              end
                              i32.const 2116560
                              call $fimport$0
                              local.set 2
                              br 11 (;@2;)
                            end
                            i32.const 2116560
                            call $fimport$0
                            local.set 2
                            br 10 (;@2;)
                          end
                          i32.const 2116560
                          call $fimport$0
                          local.set 2
                          br 9 (;@2;)
                        end
                        i32.const 2116560
                        call $fimport$0
                        local.set 2
                        br 8 (;@2;)
                      end
                      i32.const 2116560
                      call $fimport$0
                      local.set 2
                      br 7 (;@2;)
                    end
                    i32.const 2116560
                    call $fimport$0
                    local.set 2
                    br 6 (;@2;)
                  end
                  i32.const 2116560
                  call $fimport$0
                  local.set 2
                  br 5 (;@2;)
                end
                i32.const 2116560
                call $fimport$0
                local.set 2
                br 4 (;@2;)
              end
              i32.const 2116560
              call $fimport$0
              local.set 2
              br 3 (;@2;)
            end
            i32.const 2116560
            call $fimport$0
            local.set 2
            br 2 (;@2;)
          end
          i32.const 2116560
          call $fimport$0
          local.set 2
          br 1 (;@2;)
        end
        i32.const 2116560
        call $fimport$0
        local.set 2
      end
      block  ;; label = @2
        call $fimport$1
        i32.const 2116560
        call $fimport$9
        i32.ne
        br_if 0 (;@2;)
        local.get 2
        call $fimport$8
        local.set 0
        i32.const 3986628
        i32.const 0
        i32.store
        i32.const 1008
        i32.const 3431480
        call $fimport$3
        local.set 1
        i32.const 3986628
        i32.load
        local.set 2
        i32.const 3986628
        i32.const 0
        i32.store
        block  ;; label = @3
          block  ;; label = @4
            local.get 2
            i32.const 1
            i32.eq
            br_if 0 (;@4;)
            local.get 0
            i32.load
            i32.load
            local.set 2
            i32.const 3986628
            i32.const 0
            i32.store
            i32.const 1009
            local.get 1
            local.get 2
            call $fimport$2
            local.set 1
            i32.const 3986628
            i32.load
            local.set 2
            i32.const 3986628
            i32.const 0
            i32.store
            local.get 2
            i32.const 1
            i32.eq
            br_if 0 (;@4;)
            local.get 1
            if  ;; label = @5
              local.get 6
              i32.load offset=4
              local.set 1
              local.get 6
              local.get 1
              i32.const 2
              i32.shl
              i32.add
              local.get 0
              i32.load
              i32.store
              local.get 6
              local.get 1
              i32.const 1
              i32.add
              i32.store offset=4
              call $fimport$15
              i32.const 3430720
              call $j872
              call $j871
              local.get 6
              i32.load offset=4
              i32.const 2
              i32.shl
              local.get 6
              i32.add
              i32.const 4
              i32.sub
              i32.load
              i32.const 0
              call $j2180
              local.get 6
              local.get 6
              i32.load offset=4
              i32.const 1
              i32.sub
              i32.store offset=4
              br 4 (;@1;)
            end
            i32.const 4
            call $fimport$14
            local.set 1
            local.get 1
            local.get 0
            i32.load
            i32.store
            i32.const 3986628
            i32.const 0
            i32.store
            i32.const 1010
            local.get 1
            i32.const 2116560
            i32.const 0
            call $fimport$5
            i32.const 3986628
            i32.load
            local.set 0
            i32.const 3986628
            i32.const 0
            i32.store
            local.get 0
            i32.const 1
            i32.ne
            br_if 1 (;@3;)
          end
          call $fimport$4
          local.set 2
          call $fimport$1
          drop
          i32.const 3986628
          i32.const 0
          i32.store
          i32.const 391
          call $fimport$6
          i32.const 3986628
          i32.load
          local.set 0
          i32.const 3986628
          i32.const 0
          i32.store
          local.get 0
          i32.const 1
          i32.ne
          br_if 1 (;@2;)
          i32.const 0
          call $fimport$0
          drop
          call $fimport$1
          drop
          call $j894
        end
        unreachable
      end
      local.get 2
      call $fimport$10
      unreachable
    end
    local.get 6
    i32.const 16
    i32.add
    global.set 0)