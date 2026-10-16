  (func $j1804 (type 0) (param i32 i32)
    (local i32)
    i32.const 4509029
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3428556
      call $j870
      i32.const 3432052
      call $j870
      i32.const 3432076
      call $j870
      i32.const 3434152
      call $j870
      i32.const 3434312
      call $j870
      i32.const 3434604
      call $j870
      i32.const 3437580
      call $j870
      i32.const 3437592
      call $j870
      i32.const 4509029
      i32.const 1
      i32.store8
    end
    i32.const 3432052
    i32.load
    i32.load offset=92
    i32.load offset=16
    local.set 1
    i32.const 3432052
    i32.load
    i32.load offset=92
    local.get 0
    i32.store offset=16
    i32.const 3428556
    i32.load
    call $j871
    i32.const 3428556
    i32.load
    i32.load offset=92
    i32.load offset=4
    local.set 2
    local.get 2
    if  ;; label = @1
      local.get 2
      local.get 0
      i32.const 0
      call $j1552
    end
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
                        block  ;; label = @11
                          block  ;; label = @12
                            i32.const 3432052
                            i32.load
                            i32.load offset=92
                            i32.load offset=16
                            br_table 0 (;@12;) 1 (;@11;) 2 (;@10;) 11 (;@1;) 3 (;@9;) 4 (;@8;) 5 (;@7;) 6 (;@6;) 7 (;@5;) 8 (;@4;) 9 (;@3;) 10 (;@2;) 11 (;@1;)
                          end
                          i32.const 3437580
                          i32.load
                          call $j871
                          i32.const 1
                          i32.const 0
                          call $j9352
                          i32.const 3432052
                          i32.load
                          i32.load offset=92
                          i32.const 0
                          i32.store8 offset=4
                          i32.const 3434152
                          i32.load
                          call $j871
                          i32.const 103
                          i32.const 0
                          i32.const 0
                          i32.const 0
                          call $j1091
                          i32.const 3432076
                          i32.load
                          i32.load offset=92
                          i32.load
                          i32.const 2
                          i32.const 0
                          call $j3213
                          i32.const 3432076
                          i32.load
                          i32.load offset=92
                          i32.load
                          i32.const 4
                          i32.const 0
                          call $j3213
                          i32.const 3434604
                          i32.load
                          i32.load offset=92
                          i32.load
                          i32.const 1
                          i32.const 0
                          i32.const 0
                          call $j2934
                          br 10 (;@1;)
                        end
                        i32.const 3434152
                        i32.load
                        call $j871
                        i32.const 100
                        i32.const 0
                        i32.const 0
                        i32.const 0
                        call $j1091
                        i32.const 3432052
                        i32.load
                        i32.load offset=92
                        i32.const 0
                        i32.store8 offset=4
                        br 9 (;@1;)
                      end
                      i32.const 3432052
                      i32.load
                      i32.load offset=92
                      i32.const 1
                      i32.store8 offset=4
                      br 8 (;@1;)
                    end
                    i32.const 3432052
                    i32.load
                    i32.load offset=92
                    i32.const 0
                    i32.store8 offset=4
                    br 7 (;@1;)
                  end
                  i32.const 3432052
                  i32.load
                  i32.load offset=92
                  i32.const 0
                  i32.store8 offset=4
                  br 6 (;@1;)
                end
                i32.const 3434152
                i32.load
                call $j871
                i32.const 102
                i32.const 0
                i32.const 0
                i32.const 0
                call $j1091
                i32.const 3432052
                i32.load
                i32.load offset=92
                i32.const 0
                i32.store8 offset=4
                br 5 (;@1;)
              end
              i32.const 3432052
              i32.load
              i32.load offset=92
              i32.const 0
              i32.store8 offset=4
              br 4 (;@1;)
            end
            i32.const 3434152
            i32.load
            call $j871
            i32.const 105
            i32.const 0
            i32.const 0
            i32.const 0
            call $j1091
            i32.const 3432052
            i32.load
            i32.load offset=92
            i32.const 0
            i32.store8 offset=4
            br 3 (;@1;)
          end
          i32.const 3432052
          i32.load
          i32.load offset=92
          i32.const 0
          i32.store8 offset=4
          br 2 (;@1;)
        end
        i32.const 3434152
        i32.load
        call $j871
        i32.const 106
        i32.const 0
        i32.const 0
        i32.const 0
        call $j1091
        i32.const 3432052
        i32.load
        i32.load offset=92
        i32.const 0
        i32.store8 offset=4
        br 1 (;@1;)
      end
      i32.const 3434312
      i32.load
      call $j871
      block  ;; label = @2
        i32.const 3434312
        i32.load
        i32.load offset=92
        i32.load offset=12
        i32.const 6
        i32.ne
        br_if 0 (;@2;)
        i32.const 3437592
        i32.load
        call $j871
        i32.const 3437592
        i32.load
        i32.load offset=92
        i32.load offset=4
        i32.const 0
        call $j3323
        local.set 0
        local.get 0
        i32.eqz
        br_if 0 (;@2;)
        local.get 0
        i32.load offset=12
        local.set 0
        i32.const 3434152
        i32.load
        call $j871
        local.get 0
        i32.const 0
        i32.const 0
        i32.const 0
        call $j1091
      end
      i32.const 3432052
      i32.load
      i32.load offset=92
      i32.const 0
      i32.store8 offset=4
    end
    i32.const 3432052
    i32.load
    i32.load offset=92
    local.set 0
    block  ;; label = @1
      local.get 1
      br_if 0 (;@1;)
      local.get 0
      i32.load offset=16
      i32.eqz
      br_if 0 (;@1;)
      i32.const 3437580
      i32.load
      call $j871
      i32.const 2
      i32.const 0
      call $j9352
    end)
