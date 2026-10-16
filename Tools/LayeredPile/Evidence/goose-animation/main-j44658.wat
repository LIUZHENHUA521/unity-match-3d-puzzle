  (func $j44658 (type 11) (param i32 i32 i32 i32 i32 i32)
    (local f32 f32 f32)
    i32.const 4526035
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3434688
      call $j870
      i32.const 4526035
      i32.const 1
      i32.store8
    end
    local.get 4
    local.get 3
    f32.load
    f32.store
    block  ;; label = @1
      block  ;; label = @2
        block  ;; label = @3
          block  ;; label = @4
            block  ;; label = @5
              block  ;; label = @6
                block  ;; label = @7
                  local.get 1
                  br_table 4 (;@3;) 5 (;@2;) 0 (;@7;) 1 (;@6;) 6 (;@1;) 2 (;@5;) 3 (;@4;)
                end
                local.get 2
                i32.const 9702
                local.get 0
                local.get 2
                i32.load
                local.get 3
                call $redir_iiii
                i32.store
                return
              end
              local.get 2
              i32.load
              local.set 1
              local.get 1
              local.set 5
              local.get 1
              i32.load
              local.set 1
              local.get 1
              i32.load offset=620
              local.get 5
              local.get 1
              i32.const 432
              i32.add
              i32.load offset=192
              call $redir_fii
              local.set 6
              local.get 3
              f32.load
              local.set 7
              local.get 2
              i32.load
              f32.load offset=28
              local.set 8
              i32.const 3434688
              i32.load
              call $j871
              local.get 3
              local.get 7
              f32.const 0x1p+0 (;=1;)
              local.get 8
              f32.sub
              f32.abs
              local.get 6
              f32.mul
              f32.mul
              f32.store
              local.get 4
              local.get 4
              f32.load
              f32.const 0x1p+0 (;=1;)
              local.get 0
              f32.load offset=28
              f32.sub
              f32.abs
              local.get 6
              f32.mul
              f32.mul
              f32.store
              return
            end
            local.get 2
            i32.const 9702
            local.get 0
            local.get 2
            i32.load
            local.get 3
            call $redir_iiii
            i32.store
            br 3 (;@1;)
          end
          i32.const 119713
          local.get 1
          i32.const 3482164
          call $j872
          call $redir_iii
          i32.const 3482064
          call $j872
          call $j876
          unreachable
        end
        local.get 3
        f32.load
        local.set 6
        local.get 2
        i32.load
        f32.load offset=28
        local.set 7
        i32.const 3434688
        i32.load
        call $j871
        local.get 3
        local.get 6
        f32.const 0x1p+0 (;=1;)
        local.get 7
        f32.sub
        f32.abs
        f32.mul
        f32.store
        local.get 4
        local.get 4
        f32.load
        f32.const 0x1p+0 (;=1;)
        local.get 0
        f32.load offset=28
        f32.sub
        f32.abs
        f32.mul
        f32.store
      end
      return
    end
    local.get 2
    i32.load
    local.set 0
    local.get 0
    local.set 1
    local.get 0
    i32.load
    local.set 0
    local.get 0
    i32.load offset=620
    local.get 1
    local.get 0
    i32.const 432
    i32.add
    i32.load offset=192
    call $redir_fii
    local.set 6
    local.get 3
    local.get 3
    f32.load
    local.get 6
    f32.mul
    f32.store
    local.get 4
    local.get 4
    f32.load
    local.get 6
    f32.mul
    f32.store)