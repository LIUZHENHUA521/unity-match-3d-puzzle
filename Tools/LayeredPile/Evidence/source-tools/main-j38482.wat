  (func $j38482 (type 5) (param i32 i32 i32) (result i32)
    (local i32 i32 i32 i32 i32 i32 i32 f32)
    global.get 0
    i32.const 240
    i32.sub
    local.set 2
    local.get 2
    global.set 0
    local.get 2
    local.get 1
    i32.store offset=236
    i32.const 4509392
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3428556
      call $j870
      i32.const 3430028
      call $j870
      i32.const 3430384
      call $j870
      i32.const 3457648
      call $j870
      i32.const 3457660
      call $j870
      i32.const 3457612
      call $j870
      i32.const 3457616
      call $j870
      i32.const 3457664
      call $j870
      i32.const 3491016
      call $j870
      i32.const 3432084
      call $j870
      i32.const 3463820
      call $j870
      i32.const 3434152
      call $j870
      i32.const 3434312
      call $j870
      i32.const 3434332
      call $j870
      i32.const 3470236
      call $j870
      i32.const 3471620
      call $j870
      i32.const 3471632
      call $j870
      i32.const 3470256
      call $j870
      i32.const 3471608
      call $j870
      i32.const 3470264
      call $j870
      i32.const 3469508
      call $j870
      i32.const 3472460
      call $j870
      i32.const 3471676
      call $j870
      i32.const 3469512
      call $j870
      i32.const 3471680
      call $j870
      i32.const 3470268
      call $j870
      i32.const 3425124
      call $j870
      i32.const 3434516
      call $j870
      i32.const 3435048
      call $j870
      i32.const 3435224
      call $j870
      i32.const 3427840
      call $j870
      i32.const 3435416
      call $j870
      i32.const 3437580
      call $j870
      i32.const 3438836
      call $j870
      i32.const 3568632
      call $j870
      i32.const 3569744
      call $j870
      i32.const 3520228
      call $j870
      i32.const 3542308
      call $j870
      i32.const 4509392
      i32.const 1
      i32.store8
    end
    local.get 2
    i32.const 0
    i32.store offset=232
    i32.const 0
    local.set 5
    local.get 2
    i32.const 0
    i32.store offset=224
    local.get 2
    local.get 5
    i32.store offset=228
    local.get 2
    i32.const 0
    i32.store offset=216
    i32.const 0
    local.set 5
    local.get 2
    i32.const 0
    i32.store offset=208
    local.get 2
    local.get 5
    i32.store offset=212
    local.get 2
    i32.const 0
    i32.store offset=200
    i32.const 0
    local.set 5
    local.get 2
    i32.const 0
    i32.store offset=192
    local.get 2
    local.get 5
    i32.store offset=196
    local.get 2
    i32.const 0
    i32.store offset=184
    i32.const 0
    local.set 5
    local.get 2
    i32.const 0
    i32.store offset=176
    local.get 2
    local.get 5
    i32.store offset=180
    i32.const 3438836
    i32.load
    local.set 3
    local.get 2
    i32.const 160
    i32.add
    local.set 1
    local.get 1
    i32.const -1
    i32.store offset=4
    local.get 1
    local.get 3
    i32.store
    local.get 1
    local.get 2
    i32.load offset=236
    i32.store offset=8
    local.get 1
    i32.const 0
    call $j956
    local.set 1
    i32.const 3435224
    i32.load
    call $j871
    i32.const 4514392
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3435224
      call $j870
      i32.const 4514392
      i32.const 1
      i32.store8
    end
    i32.const 3435224
    i32.load
    call $j871
    i32.const 3435224
    i32.load
    i32.load offset=92
    i32.load offset=32
    i32.load offset=36
    i32.load offset=8
    local.set 3
    i32.const 3434312
    i32.load
    call $j871
    local.get 1
    local.set 6
    local.get 3
    local.set 5
    i32.const 3434312
    i32.load
    i32.load offset=92
    i32.load offset=28
    local.set 7
    local.get 0
    i32.load offset=236
    local.get 0
    i32.load offset=240
    i32.sub
    f32.convert_i32_s
    local.set 10
    i32.const 3432084
    i32.load
    local.set 3
    i32.const 3434312
    i32.load
    i32.load offset=92
    local.set 4
    local.get 2
    i32.const 144
    i32.add
    local.set 1
    local.get 1
    i32.const -1
    i32.store offset=4
    local.get 1
    local.get 3
    i32.store
    local.get 1
    local.get 4
    i32.load offset=12
    i32.store offset=8
    local.get 6
    local.get 5
    local.get 7
    local.get 10
    local.get 1
    i32.const 0
    call $j956
    i32.const 0
    call $j5164
    i32.const 0
    call $j31821
    local.get 2
    i32.load offset=236
    local.set 1
    i32.const 3434332
    i32.load
    call $j871
    block  ;; label = @1
      block  ;; label = @2
        local.get 1
        i32.const 3434332
        i32.load
        i32.load offset=92
        i32.load offset=16
        i32.load offset=16
        i32.eq
        if  ;; label = @3
          i32.const 3434312
          i32.load
          call $j871
          i32.const 3434312
          i32.load
          i32.load offset=92
          i32.load offset=40
          i32.load offset=24
          i32.const 16
          i32.add
          local.set 1
          br 1 (;@2;)
        end
        i32.const 3434332
        i32.load
        call $j871
        block  ;; label = @3
          i32.const 3434332
          i32.load
          i32.load offset=92
          i32.load offset=16
          i32.load offset=12
          i32.const 2
          i32.lt_s
          br_if 0 (;@3;)
          local.get 2
          i32.load offset=236
          local.set 1
          i32.const 3434332
          i32.load
          call $j871
          local.get 1
          i32.const 3434332
          i32.load
          i32.load offset=92
          i32.load offset=16
          i32.load offset=20
          i32.ne
          br_if 0 (;@3;)
          i32.const 3434312
          i32.load
          call $j871
          i32.const 3434312
          i32.load
          i32.load offset=92
          i32.load offset=40
          i32.load offset=24
          i32.const 20
          i32.add
          local.set 1
          br 1 (;@2;)
        end
        i32.const 3434332
        i32.load
        call $j871
        block  ;; label = @3
          i32.const 3434332
          i32.load
          i32.load offset=92
          i32.load offset=16
          i32.load offset=12
          i32.const 3
          i32.lt_s
          br_if 0 (;@3;)
          local.get 2
          i32.load offset=236
          local.set 1
          i32.const 3434332
          i32.load
          call $j871
          local.get 1
          i32.const 3434332
          i32.load
          i32.load offset=92
          i32.load offset=16
          i32.load offset=24
          i32.ne
          br_if 0 (;@3;)
          i32.const 3434312
          i32.load
          call $j871
          i32.const 3434312
          i32.load
          i32.load offset=92
          i32.load offset=40
          i32.load offset=24
          i32.const 24
          i32.add
          local.set 1
          br 1 (;@2;)
        end
        block  ;; label = @3
          block  ;; label = @4
            local.get 2
            i32.load offset=236
            local.set 1
            local.get 1
            i32.const 3
            i32.sub
            br_table 0 (;@4;) 1 (;@3;) 3 (;@1;)
          end
          i32.const 3434312
          i32.load
          call $j871
          i32.const 3434312
          i32.load
          i32.load offset=92
          i32.load offset=40
          i32.const 28
          i32.add
          local.set 1
          br 1 (;@2;)
        end
        i32.const 3434312
        i32.load
        call $j871
        i32.const 3434312
        i32.load
        i32.load offset=92
        i32.load offset=40
        i32.const 32
        i32.add
        local.set 1
      end
      local.get 1
      local.get 1
      i32.load
      i32.const 1
      i32.add
      i32.store
      local.get 2
      i32.load offset=236
      local.set 1
    end
    i32.const 1
    local.set 7
    block  ;; label = @1
      block  ;; label = @2
        block  ;; label = @3
          block  ;; label = @4
            block  ;; label = @5
              block  ;; label = @6
                block  ;; label = @7
                  block  ;; label = @8
                    local.get 1
                    br_table 0 (;@8;) 1 (;@7;) 2 (;@6;) 5 (;@3;) 6 (;@2;) 4 (;@4;) 3 (;@5;) 7 (;@1;)
                  end
                  i32.const 3437580
                  i32.load
                  call $j871
                  i32.const 24
                  i32.const 0
                  i32.const 3520228
                  i32.load
                  i32.const 0
                  call $j3492
                  local.get 0
                  local.get 0
                  call $j5870
                  i32.const 3470264
                  i32.load
                  drop
                  local.get 0
                  i32.load offset=188
                  i32.load offset=12
                  local.set 1
                  i32.const 3427840
                  i32.load
                  local.get 1
                  i32.const 3
                  local.get 1
                  i32.const 3
                  i32.lt_s
                  select
                  call $j888
                  local.set 1
                  i32.const 0
                  local.set 3
                  loop  ;; label = @8
                    local.get 1
                    i32.load offset=12
                    local.set 4
                    local.get 4
                    local.get 3
                    i32.gt_s
                    if  ;; label = @9
                      local.get 0
                      i32.load offset=188
                      local.get 3
                      i32.const 3470268
                      i32.load
                      call $j895
                      local.set 4
                      local.get 1
                      local.get 4
                      call $j877
                      local.get 1
                      local.get 3
                      i32.const 2
                      i32.shl
                      i32.add
                      local.get 4
                      i32.store offset=16
                      local.get 3
                      i32.const 1
                      i32.add
                      local.set 3
                      br 1 (;@8;)
                    end
                  end
                  i32.const 0
                  local.set 3
                  loop  ;; label = @8
                    local.get 3
                    local.get 4
                    i32.lt_s
                    if  ;; label = @9
                      local.get 0
                      i32.load offset=188
                      local.get 1
                      local.get 3
                      i32.const 2
                      i32.shl
                      i32.add
                      i32.load offset=16
                      i32.const 3470256
                      i32.load
                      call $j1342
                      drop
                      local.get 0
                      i32.load offset=192
                      local.get 1
                      local.get 3
                      i32.const 2
                      i32.shl
                      i32.add
                      i32.load offset=16
                      i32.const 3470236
                      i32.load
                      call $j903
                      block  ;; label = @10
                        local.get 0
                        i32.load8_u offset=660
                        if  ;; label = @11
                          local.get 0
                          local.get 1
                          local.get 3
                          i32.const 2
                          i32.shl
                          i32.add
                          i32.load offset=16
                          local.get 0
                          call $j19860
                          br 1 (;@10;)
                        end
                        local.get 2
                        i32.const 224
                        i32.add
                        local.get 0
                        i32.const 1
                        local.get 0
                        call $j9875
                        local.get 1
                        local.get 3
                        i32.const 2
                        i32.shl
                        i32.add
                        i32.load offset=16
                        local.set 4
                        local.get 0
                        i32.load offset=128
                        local.set 7
                        local.get 2
                        local.get 2
                        i32.load offset=232
                        i32.store offset=8
                        local.get 2
                        i32.load offset=224
                        local.set 5
                        local.get 2
                        i32.load offset=228
                        local.set 6
                        local.get 2
                        local.get 5
                        i32.store
                        local.get 2
                        local.get 6
                        i32.store offset=4
                        local.get 4
                        local.get 2
                        local.get 7
                        i32.const 0
                        call $j19258
                      end
                      local.get 3
                      i32.const 1
                      i32.add
                      local.set 3
                      local.get 1
                      i32.load offset=12
                      local.set 4
                      br 1 (;@8;)
                    end
                  end
                  block  ;; label = @8
                    local.get 0
                    i32.load8_u offset=660
                    i32.eqz
                    br_if 0 (;@8;)
                    i32.const 0
                    local.set 3
                    loop  ;; label = @9
                      local.get 3
                      local.get 4
                      i32.ge_s
                      br_if 1 (;@8;)
                      local.get 1
                      local.get 3
                      i32.const 2
                      i32.shl
                      i32.add
                      i32.load offset=16
                      local.set 4
                      local.get 2
                      i32.const 160
                      i32.add
                      local.get 0
                      i32.load offset=128
                      i32.const 0
                      call $j1387
                      local.get 4
                      local.get 2
                      f32.load offset=160
                      i32.const 0
                      call $j19259
                      local.get 3
                      i32.const 1
                      i32.add
                      local.set 3
                      local.get 1
                      i32.load offset=12
                      local.set 4
                      br 0 (;@9;)
                    end
                    unreachable
                  end
                  i32.const 3434312
                  i32.load
                  call $j871
                  local.get 0
                  call $j5871
                  if  ;; label = @8
                    i32.const 1
                    local.set 7
                    local.get 0
                    i32.load offset=628
                    i32.const 1
                    i32.const 0
                    call $j9874
                    br 7 (;@1;)
                  end
                  i32.const 1
                  local.set 7
                  local.get 0
                  i32.const 1
                  i32.const 0
                  local.get 0
                  call $j9879
                  br 6 (;@1;)
                end
                i32.const 3437580
                i32.load
                call $j871
                i32.const 25
                i32.const 0
                i32.const 3520228
                i32.load
                i32.const 0
                call $j3492
                i32.const 3470264
                i32.load
                drop
                i32.const 3472460
                i32.load
                drop
                local.get 0
                i32.load offset=188
                i32.load offset=12
                local.get 0
                i32.load offset=184
                i32.load offset=12
                i32.const 1
                i32.sub
                i32.eq
                if  ;; label = @7
                  local.get 0
                  i32.load offset=188
                  i32.const 0
                  i32.const 3470268
                  i32.load
                  call $j895
                  i32.load offset=12
                  local.set 3
                  i32.const 1
                  local.set 1
                  loop  ;; label = @8
                    i32.const 3470264
                    i32.load
                    drop
                    local.get 1
                    local.get 0
                    i32.load offset=188
                    i32.load offset=12
                    i32.lt_s
                    if  ;; label = @9
                      block  ;; label = @10
                        local.get 0
                        i32.load8_u offset=48
                        if  ;; label = @11
                          local.get 0
                          i32.load offset=80
                          local.get 0
                          i32.load offset=188
                          local.get 1
                          i32.const 3470268
                          i32.load
                          call $j895
                          i32.load offset=12
                          i32.const 3463820
                          i32.load
                          call $j1647
                          br_if 1 (;@10;)
                        end
                        local.get 3
                        local.get 0
                        i32.load offset=188
                        local.get 1
                        i32.const 3470268
                        i32.load
                        call $j895
                        i32.load offset=12
                        i32.const 0
                        call $j899
                        if  ;; label = @11
                          i32.const 3470264
                          i32.load
                          drop
                          local.get 0
                          i32.load offset=192
                          i32.load offset=12
                          local.set 1
                          loop  ;; label = @12
                            local.get 1
                            i32.const 1
                            i32.sub
                            local.set 1
                            local.get 1
                            i32.const -1
                            i32.le_s
                            br_if 11 (;@1;)
                            local.get 0
                            i32.load offset=192
                            local.get 1
                            i32.const 3470268
                            i32.load
                            call $j895
                            i32.load offset=12
                            local.get 3
                            i32.const 0
                            call $j899
                            i32.eqz
                            br_if 0 (;@12;)
                          end
                          local.get 0
                          i32.load offset=192
                          local.get 1
                          i32.const 3470268
                          i32.load
                          call $j895
                          i32.const 0
                          call $j944
                          local.set 3
                          local.get 2
                          i32.const 208
                          i32.add
                          local.get 0
                          local.get 0
                          i32.load offset=192
                          local.get 1
                          i32.const 3470268
                          i32.load
                          call $j895
                          i32.const 1
                          i32.const 0
                          local.get 0
                          call $j13227
                          local.get 2
                          local.get 2
                          i32.load offset=216
                          i32.store offset=24
                          local.get 2
                          i32.load offset=208
                          local.set 6
                          local.get 2
                          i32.load offset=212
                          local.set 5
                          local.get 2
                          local.get 6
                          i32.store offset=16
                          local.get 2
                          local.get 5
                          i32.store offset=20
                          local.get 0
                          local.get 2
                          i32.const 16
                          i32.add
                          local.get 3
                          i32.const 0
                          local.get 0
                          call $j19876
                          br 10 (;@1;)
                        end
                        local.get 0
                        i32.load offset=188
                        local.get 1
                        i32.const 3470268
                        i32.load
                        call $j895
                        i32.load offset=12
                        local.set 3
                      end
                      local.get 1
                      i32.const 1
                      i32.add
                      local.set 1
                      br 1 (;@8;)
                    end
                  end
                  i32.const 0
                  local.set 7
                  i32.const 3434152
                  i32.load
                  call $j871
                  i32.const 600
                  i32.const 3568632
                  i32.load
                  i32.const 0
                  call $j1117
                  br 6 (;@1;)
                end
                i32.const 3425124
                i32.load
                call $j873
                local.set 3
                local.get 3
                i32.const 3471608
                i32.load
                call $j918
                i32.const 0
                local.set 1
                loop  ;; label = @7
                  i32.const 3470264
                  i32.load
                  drop
                  local.get 1
                  local.get 0
                  i32.load offset=188
                  i32.load offset=12
                  i32.lt_s
                  if  ;; label = @8
                    block  ;; label = @9
                      local.get 0
                      i32.load8_u offset=48
                      if  ;; label = @10
                        local.get 0
                        i32.load offset=80
                        local.get 0
                        i32.load offset=188
                        local.get 1
                        i32.const 3470268
                        i32.load
                        call $j895
                        i32.load offset=12
                        i32.const 3463820
                        i32.load
                        call $j1647
                        br_if 1 (;@9;)
                      end
                      local.get 3
                      local.get 0
                      i32.load offset=188
                      local.get 1
                      i32.const 3470268
                      i32.load
                      call $j895
                      i32.load offset=12
                      i32.const 3471632
                      i32.load
                      call $j1235
                      br_if 0 (;@9;)
                      local.get 3
                      local.get 0
                      i32.load offset=188
                      local.get 1
                      i32.const 3470268
                      i32.load
                      call $j895
                      i32.load offset=12
                      i32.const 3471620
                      i32.load
                      call $j903
                    end
                    local.get 1
                    i32.const 1
                    i32.add
                    local.set 1
                    br 1 (;@7;)
                  end
                end
                i32.const 3471676
                i32.load
                drop
                local.get 3
                i32.const 0
                local.get 3
                i32.load offset=12
                i32.const 0
                call $j1595
                i32.const 3471680
                i32.load
                call $j895
                local.set 8
                i32.const 0
                local.set 1
                i32.const 0
                local.set 3
                loop  ;; label = @7
                  i32.const 3470264
                  i32.load
                  drop
                  local.get 1
                  local.get 0
                  i32.load offset=188
                  i32.load offset=12
                  i32.lt_s
                  if  ;; label = @8
                    local.get 3
                    i32.const 1
                    i32.add
                    local.get 3
                    local.get 8
                    local.get 0
                    i32.load offset=188
                    local.get 1
                    i32.const 3470268
                    i32.load
                    call $j895
                    i32.load offset=12
                    i32.const 0
                    call $j899
                    select
                    local.set 3
                    local.get 1
                    i32.const 1
                    i32.add
                    local.set 1
                    br 1 (;@7;)
                  end
                end
                i32.const 0
                local.set 4
                loop  ;; label = @7
                  local.get 4
                  i32.const 3
                  local.get 3
                  i32.sub
                  i32.ge_s
                  br_if 6 (;@1;)
                  i32.const 3470264
                  i32.load
                  drop
                  local.get 0
                  i32.load offset=192
                  i32.load offset=12
                  local.set 1
                  block  ;; label = @8
                    loop  ;; label = @9
                      local.get 1
                      i32.const 1
                      i32.sub
                      local.set 1
                      local.get 1
                      i32.const -1
                      i32.le_s
                      br_if 1 (;@8;)
                      local.get 0
                      i32.load offset=192
                      local.get 1
                      i32.const 3470268
                      i32.load
                      call $j895
                      i32.load offset=12
                      local.get 8
                      i32.const 0
                      call $j899
                      i32.eqz
                      br_if 0 (;@9;)
                    end
                    local.get 0
                    i32.load offset=192
                    local.get 1
                    i32.const 3470268
                    i32.load
                    call $j895
                    i32.const 0
                    call $j944
                    local.set 9
                    local.get 2
                    i32.const 192
                    i32.add
                    local.get 0
                    local.get 0
                    i32.load offset=192
                    local.get 1
                    i32.const 3470268
                    i32.load
                    call $j895
                    i32.const 1
                    i32.const 0
                    local.get 0
                    call $j13227
                    local.get 2
                    local.get 2
                    i32.load offset=200
                    i32.store offset=40
                    local.get 2
                    i32.load offset=192
                    local.set 5
                    local.get 2
                    i32.load offset=196
                    local.set 6
                    local.get 2
                    local.get 5
                    i32.store offset=32
                    local.get 2
                    local.get 6
                    i32.store offset=36
                    local.get 0
                    local.get 2
                    i32.const 32
                    i32.add
                    local.get 9
                    local.get 4
                    local.get 0
                    call $j19876
                  end
                  local.get 4
                  i32.const 1
                  i32.add
                  local.set 4
                  br 0 (;@7;)
                end
                unreachable
              end
              block  ;; label = @6
                local.get 0
                i32.load offset=712
                i32.const 2
                i32.const 3457648
                i32.load
                call $j1065
                i32.eqz
                br_if 0 (;@6;)
                local.get 0
                i32.load offset=716
                local.get 2
                i32.load offset=236
                i32.const 3457612
                i32.load
                call $j1065
                i32.eqz
                br_if 0 (;@6;)
                local.get 0
                i32.load offset=708
                local.get 2
                i32.load offset=236
                i32.const 3457660
                i32.load
                call $j1065
                i32.eqz
                br_if 0 (;@6;)
                local.get 0
                i32.load offset=716
                local.get 2
                i32.load offset=236
                i32.const 3457616
                i32.load
                call $j1060
                i32.const 0
                call $j1027
                local.get 0
                i32.load offset=708
                local.get 2
                i32.load offset=236
                i32.const 3457664
                i32.load
                call $j1060
                i32.const 0
                call $j1628
                local.get 0
                i32.load offset=716
                local.get 2
                i32.load offset=236
                i32.const 3457616
                i32.load
                call $j1060
                i32.const 0
                call $j1027
                local.set 1
                i32.const 4525920
                i32.load8_u
                i32.eqz
                if  ;; label = @7
                  i32.const 3439908
                  call $j870
                  i32.const 4525920
                  i32.const 1
                  i32.store8
                end
                i32.const 3439908
                i32.load
                i32.load offset=92
                local.set 3
                local.get 3
                i32.load
                local.set 6
                local.get 3
                i32.load offset=4
                local.set 5
                local.get 2
                local.get 3
                i32.load offset=8
                i32.store offset=136
                local.get 2
                local.get 2
                i32.load offset=136
                i32.store offset=88
                local.get 2
                local.get 6
                i32.store offset=128
                local.get 2
                local.get 5
                i32.store offset=132
                local.get 2
                i32.load offset=128
                local.set 5
                local.get 2
                i32.load offset=132
                local.set 6
                local.get 2
                local.get 5
                i32.store offset=80
                local.get 2
                local.get 6
                i32.store offset=84
                local.get 1
                local.get 2
                i32.const 80
                i32.add
                i32.const 0
                call $j1271
                local.get 0
                i32.load offset=716
                local.get 2
                i32.load offset=236
                i32.const 3457616
                i32.load
                call $j1060
                i32.const 0
                call $j1027
                local.set 1
                i32.const 4525920
                i32.load8_u
                i32.eqz
                if  ;; label = @7
                  i32.const 3439908
                  call $j870
                  i32.const 4525920
                  i32.const 1
                  i32.store8
                end
                i32.const 3439908
                i32.load
                i32.load offset=92
                local.set 3
                local.get 3
                i32.load
                local.set 6
                local.get 3
                i32.load offset=4
                local.set 5
                local.get 2
                local.get 3
                i32.load offset=8
                i32.store offset=120
                local.get 2
                local.get 2
                i32.load offset=120
                i32.store offset=72
                local.get 2
                local.get 6
                i32.store offset=112
                local.get 2
                local.get 5
                i32.store offset=116
                local.get 2
                i32.load offset=112
                local.set 5
                local.get 2
                i32.load offset=116
                local.set 6
                local.get 2
                local.get 5
                i32.store offset=64
                local.get 2
                local.get 6
                i32.store offset=68
                local.get 1
                local.get 2
                i32.const -64
                i32.sub
                i32.const 0
                call $j1725
                local.get 0
                i32.load offset=716
                local.get 2
                i32.load offset=236
                i32.const 3457616
                i32.load
                call $j1060
                i32.const 0
                call $j1027
                local.set 1
                i32.const 4518492
                i32.load8_u
                i32.eqz
                if  ;; label = @7
                  i32.const 3439908
                  call $j870
                  i32.const 4518492
                  i32.const 1
                  i32.store8
                end
                i32.const 3439908
                i32.load
                i32.load offset=92
                local.set 3
                local.get 2
                local.get 3
                i32.load offset=20
                i32.store offset=56
                local.get 3
                i32.load offset=12
                local.set 6
                local.get 3
                i32.load offset=16
                local.set 5
                local.get 2
                local.get 6
                i32.store offset=48
                local.get 2
                local.get 5
                i32.store offset=52
                local.get 1
                local.get 2
                i32.const 48
                i32.add
                i32.const 0
                call $j1161
                local.get 0
                i32.load offset=716
                local.get 2
                i32.load offset=236
                i32.const 3457616
                i32.load
                call $j1060
                i32.const 1
                i32.const 0
                call $j907
                local.get 0
                i32.load offset=716
                local.get 2
                i32.load offset=236
                i32.const 3457616
                i32.load
                call $j1060
                i32.const 3491016
                i32.load
                call $j1083
                i32.const 0
                call $j3343
              end
              local.get 0
              local.get 0
              local.get 0
              call $j38495
              i32.const 0
              call $j1135
              drop
              br 4 (;@1;)
            end
            i32.const 3437580
            i32.load
            call $j871
            i32.const 35
            i32.const 0
            i32.const 3520228
            i32.load
            i32.const 0
            call $j3492
            local.get 0
            local.get 0
            call $j38485
            br 3 (;@1;)
          end
          local.get 0
          i32.load offset=720
          local.set 1
          i32.const 3435416
          i32.load
          call $j871
          local.get 1
          i32.const 0
          i32.const 0
          call $j924
          if  ;; label = @4
            i32.const 3434152
            i32.load
            call $j871
            i32.const 600
            i32.const 3569744
            i32.load
            i32.const 0
            call $j1117
            i32.const 0
            local.set 7
            br 3 (;@1;)
          end
          local.get 0
          local.get 0
          local.get 0
          call $j38519
          i32.const 0
          call $j1135
          drop
          br 2 (;@1;)
        end
        i32.const 3430028
        i32.load
        call $j871
        i32.const 3542308
        i32.load
        i32.const 0
        call $j4354
        i32.load offset=28
        i32.const 60
        i32.mul
        local.set 1
        i32.const 3430384
        i32.load
        call $j871
        block  ;; label = @3
          i32.const 0
          call $j13853
          if  ;; label = @4
            i32.const 3430384
            i32.load
            call $j871
            i32.const 0
            call $j6856
            i32.eqz
            br_if 1 (;@3;)
            i32.const 3430384
            i32.load
            call $j871
            local.get 1
            local.set 6
            i32.const 0
            call $j6856
            i32.load offset=36
            local.set 1
            local.get 6
            local.get 1
            local.get 1
            i32.const 1
            i32.lt_s
            select
            local.set 1
            br 1 (;@3;)
          end
          i32.const 3435048
          i32.load
          call $j871
          i32.const 0
          call $j13077
          if  ;; label = @4
            i32.const 3435048
            i32.load
            call $j871
            i32.const 4509456
            i32.load8_u
            i32.eqz
            if  ;; label = @5
              i32.const 3435048
              call $j870
              i32.const 4509456
              i32.const 1
              i32.store8
            end
            i32.const 3435048
            i32.load
            call $j871
            i32.const 3435048
            i32.load
            i32.load offset=92
            i32.load offset=12
            i32.eqz
            br_if 1 (;@3;)
            i32.const 3435048
            i32.load
            call $j871
            i32.const 4509456
            i32.load8_u
            i32.eqz
            if  ;; label = @5
              i32.const 3435048
              call $j870
              i32.const 4509456
              i32.const 1
              i32.store8
            end
            i32.const 3435048
            i32.load
            call $j871
            local.get 1
            local.set 6
            i32.const 3435048
            i32.load
            i32.load offset=92
            i32.load offset=12
            i32.load offset=36
            local.set 1
            local.get 6
            local.get 1
            local.get 1
            i32.const 1
            i32.lt_s
            select
            local.set 1
            br 1 (;@3;)
          end
          i32.const 3434516
          i32.load
          call $j871
          i32.const 0
          call $j13185
          if  ;; label = @4
            i32.const 3434516
            i32.load
            call $j871
            i32.const 3434516
            i32.load
            i32.load offset=92
            i32.load offset=20
            i32.eqz
            br_if 1 (;@3;)
            i32.const 3434516
            i32.load
            call $j871
            local.get 1
            local.set 6
            i32.const 3434516
            i32.load
            i32.load offset=92
            i32.load offset=20
            i32.load offset=40
            local.set 1
            local.get 6
            local.get 1
            local.get 1
            i32.const 1
            i32.lt_s
            select
            local.set 1
            br 1 (;@3;)
          end
          local.get 0
          i32.load offset=88
          local.set 3
          local.get 3
          i32.eqz
          br_if 0 (;@3;)
          i32.const 3469508
          i32.load
          drop
          local.get 3
          i32.load offset=12
          i32.const 1
          i32.lt_s
          br_if 0 (;@3;)
          local.get 3
          i32.const 0
          i32.const 3469512
          i32.load
          call $j895
          i32.load offset=72
          i32.const 1
          i32.lt_s
          br_if 0 (;@3;)
          local.get 3
          i32.const 0
          i32.const 3469512
          i32.load
          call $j895
          i32.load offset=72
          i32.const 60
          i32.mul
          local.set 1
        end
        local.get 0
        local.get 1
        local.get 0
        i32.load offset=240
        i32.add
        i32.store offset=240
        i32.const 3428556
        i32.load
        call $j871
        i32.const 3428556
        i32.load
        i32.load offset=92
        i32.load offset=8
        local.set 1
        local.get 1
        if  ;; label = @3
          local.get 1
          local.get 0
          i32.load offset=240
          i32.const 0
          call $j1552
        end
        i32.const 3434312
        i32.load
        call $j871
        i32.const 3434312
        i32.load
        i32.load offset=92
        i32.const 0
        i32.store8 offset=36
        br 1 (;@1;)
      end
      i32.const 3437580
      i32.load
      call $j871
      i32.const 24
      i32.const 0
      i32.const 3520228
      i32.load
      i32.const 0
      call $j3492
      local.get 0
      local.get 0
      call $j5870
      i32.const 3470264
      i32.load
      drop
      i32.const 3427840
      i32.load
      local.get 0
      i32.load offset=188
      i32.load offset=12
      call $j888
      local.set 1
      i32.const 0
      local.set 3
      loop  ;; label = @2
        local.get 1
        i32.load offset=12
        local.set 4
        local.get 4
        local.get 3
        i32.gt_s
        if  ;; label = @3
          local.get 0
          i32.load offset=188
          local.get 3
          i32.const 3470268
          i32.load
          call $j895
          local.set 4
          local.get 1
          local.get 4
          call $j877
          local.get 1
          local.get 3
          i32.const 2
          i32.shl
          i32.add
          local.get 4
          i32.store offset=16
          local.get 3
          i32.const 1
          i32.add
          local.set 3
          br 1 (;@2;)
        end
      end
      i32.const 0
      local.set 3
      loop  ;; label = @2
        local.get 3
        local.get 4
        i32.lt_s
        if  ;; label = @3
          local.get 0
          i32.load offset=188
          local.get 1
          local.get 3
          i32.const 2
          i32.shl
          i32.add
          i32.load offset=16
          i32.const 3470256
          i32.load
          call $j1342
          drop
          local.get 0
          i32.load offset=192
          local.get 1
          local.get 3
          i32.const 2
          i32.shl
          i32.add
          i32.load offset=16
          i32.const 3470236
          i32.load
          call $j903
          block  ;; label = @4
            local.get 0
            i32.load8_u offset=660
            if  ;; label = @5
              local.get 0
              local.get 1
              local.get 3
              i32.const 2
              i32.shl
              i32.add
              i32.load offset=16
              local.get 0
              call $j19860
              br 1 (;@4;)
            end
            local.get 2
            i32.const 176
            i32.add
            local.get 0
            i32.const 1
            local.get 0
            call $j9875
            local.get 1
            local.get 3
            i32.const 2
            i32.shl
            i32.add
            i32.load offset=16
            local.set 4
            local.get 0
            i32.load offset=128
            local.set 7
            local.get 2
            local.get 2
            i32.load offset=184
            i32.store offset=104
            local.get 2
            i32.load offset=176
            local.set 5
            local.get 2
            i32.load offset=180
            local.set 6
            local.get 2
            local.get 5
            i32.store offset=96
            local.get 2
            local.get 6
            i32.store offset=100
            local.get 4
            local.get 2
            i32.const 96
            i32.add
            local.get 7
            i32.const 0
            call $j19258
          end
          local.get 3
          i32.const 1
          i32.add
          local.set 3
          local.get 1
          i32.load offset=12
          local.set 4
          br 1 (;@2;)
        end
      end
      block  ;; label = @2
        local.get 0
        i32.load8_u offset=660
        i32.eqz
        br_if 0 (;@2;)
        i32.const 0
        local.set 3
        loop  ;; label = @3
          local.get 3
          local.get 4
          i32.ge_s
          br_if 1 (;@2;)
          local.get 1
          local.get 3
          i32.const 2
          i32.shl
          i32.add
          i32.load offset=16
          local.set 4
          local.get 2
          i32.const 160
          i32.add
          local.get 0
          i32.load offset=128
          i32.const 0
          call $j1387
          local.get 4
          local.get 2
          f32.load offset=160
          i32.const 0
          call $j19259
          local.get 3
          i32.const 1
          i32.add
          local.set 3
          local.get 1
          i32.load offset=12
          local.set 4
          br 0 (;@3;)
        end
        unreachable
      end
      i32.const 3434312
      i32.load
      call $j871
      block  ;; label = @2
        local.get 0
        call $j5871
        if  ;; label = @3
          local.get 0
          i32.load offset=628
          i32.const 1
          i32.const 0
          call $j9874
          br 1 (;@2;)
        end
        local.get 0
        i32.const 1
        i32.const 0
        local.get 0
        call $j9879
      end
      i32.const 3434312
      i32.load
      call $j871
      i32.const 3434312
      i32.load
      i32.load offset=92
      i32.const 0
      i32.store8 offset=36
      i32.const 1
      local.set 7
    end
    local.get 2
    i32.const 240
    i32.add
    global.set 0
    local.get 7
    return)
