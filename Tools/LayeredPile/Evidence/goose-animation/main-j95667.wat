  (func $j95667 (type 0) (param i32 i32)
    (local i32 i32 i32 i32)
    global.get 0
    i32.const 96
    i32.sub
    local.set 1
    local.get 1
    global.set 0
    i32.const 4508665
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3428672
      call $j870
      i32.const 3428772
      call $j870
      i32.const 3451048
      call $j870
      i32.const 3461200
      call $j870
      i32.const 3461204
      call $j870
      i32.const 3461208
      call $j870
      i32.const 3464936
      call $j870
      i32.const 3464940
      call $j870
      i32.const 3520228
      call $j870
      i32.const 4508665
      i32.const 1
      i32.store8
    end
    i32.const 0
    local.set 2
    local.get 1
    i32.const 0
    i32.store offset=88
    local.get 1
    local.get 2
    i32.store offset=92
    i32.const 0
    local.set 2
    local.get 1
    i32.const 0
    i32.store offset=80
    local.get 1
    local.get 2
    i32.store offset=84
    i32.const 0
    local.set 2
    local.get 1
    i32.const 0
    i32.store offset=72
    local.get 1
    local.get 2
    i32.store offset=76
    i32.const 0
    local.set 2
    local.get 1
    i32.const 0
    i32.store offset=64
    local.get 1
    local.get 2
    i32.store offset=68
    local.get 1
    i32.const 0
    i32.store offset=60
    local.get 1
    i32.const 0
    i32.store offset=56
    local.get 1
    i32.const 32
    i32.add
    local.get 0
    i32.load offset=44
    i32.const 3451048
    i32.load
    call $j1277
    local.get 1
    i32.load offset=48
    local.set 2
    local.get 1
    i32.load offset=52
    local.set 0
    local.get 1
    local.get 2
    i32.store offset=88
    local.get 1
    local.get 0
    i32.store offset=92
    local.get 1
    i32.load offset=40
    local.set 0
    local.get 1
    i32.load offset=44
    local.set 2
    local.get 1
    local.get 0
    i32.store offset=80
    local.get 1
    local.get 2
    i32.store offset=84
    local.get 1
    i32.load offset=32
    local.set 2
    local.get 1
    i32.load offset=36
    local.set 0
    local.get 1
    local.get 2
    i32.store offset=72
    local.get 1
    local.get 0
    i32.store offset=76
    local.get 1
    local.get 1
    i32.const 72
    i32.add
    i32.store offset=32
    local.get 1
    i32.const 0
    i32.store offset=24
    local.get 1
    local.get 1
    i32.load offset=32
    i32.store offset=28
    block  ;; label = @1
      block  ;; label = @2
        block  ;; label = @3
          block  ;; label = @4
            block  ;; label = @5
              block  ;; label = @6
                block  ;; label = @7
                  block  ;; label = @8
                    block  ;; label = @9
                      block  ;; label = @10
                        loop  ;; label = @11
                          i32.const 3986628
                          i32.const 0
                          i32.store
                          i32.const 5994
                          local.get 1
                          i32.const 72
                          i32.add
                          i32.const 3461204
                          i32.load
                          call $fimport$2
                          local.set 0
                          i32.const 3986628
                          i32.load
                          local.set 2
                          i32.const 3986628
                          i32.const 0
                          i32.store
                          local.get 2
                          i32.const 1
                          i32.eq
                          br_if 7 (;@4;)
                          local.get 0
                          i32.eqz
                          br_if 9 (;@2;)
                          i32.const 3986628
                          i32.const 0
                          i32.store
                          i32.const 5995
                          local.get 1
                          i32.const 32
                          i32.add
                          local.get 1
                          i32.const 72
                          i32.add
                          i32.const 3461208
                          i32.load
                          call $fimport$5
                          i32.const 3986628
                          i32.load
                          local.set 0
                          i32.const 3986628
                          i32.const 0
                          i32.store
                          block  ;; label = @12
                            block  ;; label = @13
                              block  ;; label = @14
                                block  ;; label = @15
                                  block  ;; label = @16
                                    local.get 0
                                    i32.const 1
                                    i32.ne
                                    if  ;; label = @17
                                      local.get 1
                                      i32.load offset=32
                                      local.set 0
                                      local.get 1
                                      i32.load offset=36
                                      local.set 2
                                      local.get 1
                                      local.get 0
                                      i32.store offset=64
                                      local.get 1
                                      local.get 2
                                      i32.store offset=68
                                      i32.const 3986628
                                      i32.const 0
                                      i32.store
                                      i32.const 5996
                                      local.get 1
                                      i32.const -64
                                      i32.sub
                                      i32.const 3464940
                                      i32.load
                                      call $fimport$2
                                      local.set 2
                                      i32.const 3986628
                                      i32.load
                                      local.set 0
                                      i32.const 3986628
                                      i32.const 0
                                      i32.store
                                      local.get 0
                                      i32.const 1
                                      i32.eq
                                      br_if 1 (;@16;)
                                      i32.const 0
                                      local.set 0
                                      local.get 2
                                      i32.const 1
                                      i32.gt_s
                                      br_if 5 (;@12;)
                                      i32.const 3986628
                                      i32.const 0
                                      i32.store
                                      i32.const 5997
                                      local.get 1
                                      i32.const -64
                                      i32.sub
                                      i32.const 3464936
                                      i32.load
                                      call $fimport$2
                                      local.set 0
                                      i32.const 3986628
                                      i32.load
                                      local.set 2
                                      i32.const 3986628
                                      i32.const 0
                                      i32.store
                                      local.get 2
                                      i32.const 1
                                      i32.eq
                                      br_if 2 (;@15;)
                                      local.get 1
                                      local.get 0
                                      i32.store offset=60
                                      i32.const 3428672
                                      i32.load
                                      local.set 0
                                      local.get 1
                                      i32.const -1
                                      i32.store offset=36
                                      local.get 1
                                      local.get 0
                                      i32.store offset=32
                                      local.get 1
                                      local.get 1
                                      i32.load offset=60
                                      i32.store offset=40
                                      i32.const 3986628
                                      i32.const 0
                                      i32.store
                                      i32.const 1401
                                      local.get 1
                                      i32.const 32
                                      i32.add
                                      i32.const 0
                                      call $fimport$2
                                      local.set 0
                                      i32.const 3986628
                                      i32.load
                                      local.set 2
                                      i32.const 3986628
                                      i32.const 0
                                      i32.store
                                      local.get 2
                                      i32.const 1
                                      i32.eq
                                      br_if 3 (;@14;)
                                      i32.const 3986628
                                      i32.const 0
                                      i32.store
                                      i32.const 5997
                                      local.get 1
                                      i32.const -64
                                      i32.sub
                                      i32.const 3464936
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
                                      br_if 4 (;@13;)
                                      local.get 1
                                      local.get 2
                                      i32.store offset=60
                                      i32.const 3428672
                                      i32.load
                                      local.set 2
                                      local.get 1
                                      i32.const -1
                                      i32.store offset=12
                                      local.get 1
                                      local.get 2
                                      i32.store offset=8
                                      local.get 1
                                      local.get 1
                                      i32.load offset=60
                                      i32.store offset=16
                                      i32.const 3986628
                                      i32.const 0
                                      i32.store
                                      i32.const 1401
                                      local.get 1
                                      i32.const 8
                                      i32.add
                                      i32.const 0
                                      call $fimport$2
                                      local.set 2
                                      i32.const 3986628
                                      i32.load
                                      local.set 3
                                      i32.const 3986628
                                      i32.const 0
                                      i32.store
                                      block  ;; label = @18
                                        local.get 3
                                        i32.const 1
                                        i32.eq
                                        br_if 0 (;@18;)
                                        i32.const 3986628
                                        i32.const 0
                                        i32.store
                                        i32.const 1003
                                        i32.const 3428772
                                        i32.load
                                        call $fimport$11
                                        i32.const 3986628
                                        i32.load
                                        local.set 3
                                        i32.const 3986628
                                        i32.const 0
                                        i32.store
                                        local.get 3
                                        i32.const 1
                                        i32.eq
                                        br_if 0 (;@18;)
                                        i32.const 3986628
                                        i32.const 0
                                        i32.store
                                        i32.const 5998
                                        local.get 0
                                        local.get 2
                                        local.get 1
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
                                        br_if 7 (;@11;)
                                      end
                                      i32.const 2116560
                                      call $fimport$0
                                      local.set 0
                                      br 14 (;@3;)
                                    end
                                    i32.const 2116560
                                    call $fimport$0
                                    local.set 0
                                    br 13 (;@3;)
                                  end
                                  i32.const 2116560
                                  call $fimport$0
                                  local.set 0
                                  br 12 (;@3;)
                                end
                                i32.const 2116560
                                call $fimport$0
                                local.set 0
                                br 11 (;@3;)
                              end
                              i32.const 2116560
                              call $fimport$0
                              local.set 0
                              br 10 (;@3;)
                            end
                            i32.const 2116560
                            call $fimport$0
                            local.set 0
                            br 9 (;@3;)
                          end
                          loop  ;; label = @12
                            i32.const 3986628
                            i32.const 0
                            i32.store
                            i32.const 5996
                            local.get 1
                            i32.const -64
                            i32.sub
                            i32.const 3464940
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
                            br_if 7 (;@5;)
                            local.get 0
                            local.get 2
                            i32.ge_s
                            br_if 1 (;@11;)
                            i32.const 3986628
                            i32.const 0
                            i32.store
                            i32.const 5997
                            local.get 1
                            i32.const -64
                            i32.sub
                            i32.const 3464936
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
                            if  ;; label = @13
                              local.get 1
                              local.get 2
                              i32.store offset=60
                              i32.const 3428672
                              i32.load
                              local.set 2
                              local.get 1
                              i32.const -1
                              i32.store offset=36
                              local.get 1
                              local.get 2
                              i32.store offset=32
                              local.get 1
                              local.get 1
                              i32.load offset=60
                              i32.store offset=40
                              i32.const 3986628
                              i32.const 0
                              i32.store
                              i32.const 1401
                              local.get 1
                              i32.const 32
                              i32.add
                              i32.const 0
                              call $fimport$2
                              local.set 3
                              i32.const 3986628
                              i32.load
                              local.set 2
                              i32.const 3986628
                              i32.const 0
                              i32.store
                              local.get 2
                              i32.const 1
                              i32.eq
                              br_if 3 (;@10;)
                              i32.const 3986628
                              i32.const 0
                              i32.store
                              i32.const 5997
                              local.get 1
                              i32.const -64
                              i32.sub
                              i32.const 3464936
                              i32.load
                              call $fimport$2
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
                              br_if 4 (;@9;)
                              local.get 1
                              local.get 2
                              i32.store offset=60
                              i32.const 3428672
                              i32.load
                              local.set 2
                              local.get 1
                              i32.const -1
                              i32.store offset=12
                              local.get 1
                              local.get 2
                              i32.store offset=8
                              local.get 1
                              local.get 1
                              i32.load offset=60
                              i32.store offset=16
                              i32.const 3986628
                              i32.const 0
                              i32.store
                              i32.const 1401
                              local.get 1
                              i32.const 8
                              i32.add
                              i32.const 0
                              call $fimport$2
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
                              br_if 5 (;@8;)
                              i32.const 3986628
                              i32.const 0
                              i32.store
                              local.get 1
                              local.get 0
                              i32.const 1
                              i32.add
                              i32.store offset=56
                              i32.const 1842
                              local.get 1
                              i32.const 56
                              i32.add
                              i32.const 0
                              call $fimport$2
                              local.set 4
                              i32.const 3986628
                              i32.load
                              local.set 5
                              i32.const 3986628
                              i32.const 0
                              i32.store
                              local.get 5
                              i32.const 1
                              i32.eq
                              br_if 6 (;@7;)
                              i32.const 3986628
                              i32.const 0
                              i32.store
                              local.get 2
                              local.set 5
                              i32.const 3520228
                              i32.load
                              local.set 2
                              i32.const 5493
                              local.get 5
                              local.get 4
                              local.get 2
                              local.get 2
                              local.get 2
                              local.get 2
                              i32.const 0
                              call $fimport$26
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
                              br_if 7 (;@6;)
                              i32.const 3986628
                              i32.const 0
                              i32.store
                              i32.const 1003
                              i32.const 3428772
                              i32.load
                              call $fimport$11
                              i32.const 3986628
                              i32.load
                              local.set 4
                              i32.const 3986628
                              i32.const 0
                              i32.store
                              local.get 4
                              i32.const 1
                              i32.eq
                              br_if 7 (;@6;)
                              i32.const 3986628
                              i32.const 0
                              i32.store
                              i32.const 5998
                              local.get 3
                              local.get 2
                              local.get 1
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
                              br_if 7 (;@6;)
                              local.get 0
                              i32.const 1
                              i32.add
                              local.set 0
                              br 1 (;@12;)
                            end
                          end
                        end
                        i32.const 2116560
                        call $fimport$0
                        local.set 0
                        br 7 (;@3;)
                      end
                      i32.const 2116560
                      call $fimport$0
                      local.set 0
                      br 6 (;@3;)
                    end
                    i32.const 2116560
                    call $fimport$0
                    local.set 0
                    br 5 (;@3;)
                  end
                  i32.const 2116560
                  call $fimport$0
                  local.set 0
                  br 4 (;@3;)
                end
                i32.const 2116560
                call $fimport$0
                local.set 0
                br 3 (;@3;)
              end
              i32.const 2116560
              call $fimport$0
              local.set 0
              br 2 (;@3;)
            end
            i32.const 2116560
            call $fimport$0
            local.set 0
            br 1 (;@3;)
          end
          i32.const 2116560
          call $fimport$0
          local.set 0
        end
        call $fimport$1
        i32.const 2116560
        call $fimport$9
        i32.eq
        if  ;; label = @3
          local.get 1
          local.get 0
          call $fimport$8
          i32.load
          i32.store offset=24
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
          call $fimport$4
          local.set 0
          call $fimport$1
          drop
        end
        i32.const 3986628
        i32.const 0
        i32.store
        i32.const 5999
        local.get 1
        i32.const 24
        i32.add
        call $fimport$3
        drop
        i32.const 3986628
        i32.load
        local.set 1
        i32.const 3986628
        i32.const 0
        i32.store
        local.get 1
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
      i32.const 24
      i32.add
      call $j25830
      drop
      local.get 1
      i32.const 96
      i32.add
      global.set 0
      return
    end
    local.get 0
    call $fimport$10
    unreachable)