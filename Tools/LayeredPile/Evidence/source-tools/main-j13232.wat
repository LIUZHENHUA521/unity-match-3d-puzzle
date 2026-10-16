  (func $j13232 (type 5) (param i32 i32 i32) (result i32)
    (local i32)
    global.get 0
    i32.const 16
    i32.sub
    local.set 3
    local.get 3
    global.set 0
    i32.const 4509391
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3457596
      call $j870
      i32.const 3463820
      call $j870
      i32.const 3434152
      call $j870
      i32.const 3434332
      call $j870
      i32.const 3470264
      call $j870
      i32.const 3472460
      call $j870
      i32.const 3471676
      call $j870
      i32.const 3470268
      call $j870
      i32.const 3439168
      call $j870
      i32.const 3568176
      call $j870
      i32.const 3568632
      call $j870
      i32.const 3567340
      call $j870
      i32.const 3568232
      call $j870
      i32.const 3566384
      call $j870
      i32.const 3565672
      call $j870
      i32.const 4509391
      i32.const 1
      i32.store8
    end
    i32.const 3434332
    i32.load
    call $j871
    local.get 3
    i32.const 8
    i32.add
    i32.const 0
    call $j38466
    block  ;; label = @1
      block  ;; label = @2
        block  ;; label = @3
          local.get 3
          i32.load offset=8
          local.get 1
          i32.const 3457596
          i32.load
          call $j8243
          br_if 0 (;@3;)
          i32.const 3439168
          i32.load
          i32.load offset=92
          i32.load8_u offset=8
          br_if 0 (;@3;)
          i32.const 3565672
          local.set 1
          br 1 (;@2;)
        end
        i32.const 1
        local.set 2
        block  ;; label = @3
          block  ;; label = @4
            block  ;; label = @5
              local.get 1
              br_table 0 (;@5;) 1 (;@4;) 4 (;@1;) 4 (;@1;) 4 (;@1;) 2 (;@3;) 4 (;@1;)
            end
            local.get 0
            i32.const 1
            local.get 0
            call $j13229
            i32.const 1
            i32.ge_s
            br_if 3 (;@1;)
            i32.const 3568176
            local.set 1
            br 2 (;@2;)
          end
          local.get 0
          i32.const 1
          local.get 0
          call $j13229
          i32.const 0
          i32.le_s
          if  ;; label = @4
            i32.const 3567340
            local.set 1
            local.get 0
            i32.const 1
            local.get 0
            call $j13229
            i32.const 1
            i32.lt_s
            br_if 2 (;@2;)
            i32.const 3566384
            local.set 1
            local.get 0
            i32.load8_u offset=48
            br_if 2 (;@2;)
            i32.const 0
            local.set 2
            br 3 (;@1;)
          end
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
          i32.ne
          br_if 2 (;@1;)
          local.get 0
          i32.load offset=188
          i32.const 0
          i32.const 3470268
          i32.load
          call $j895
          i32.load offset=12
          local.set 2
          i32.const 1
          local.set 1
          loop  ;; label = @4
            i32.const 3470264
            i32.load
            drop
            local.get 1
            local.get 0
            i32.load offset=188
            i32.load offset=12
            i32.lt_s
            if  ;; label = @5
              block  ;; label = @6
                local.get 0
                i32.load8_u offset=48
                if  ;; label = @7
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
                  br_if 1 (;@6;)
                end
                local.get 2
                local.get 0
                i32.load offset=188
                local.get 1
                i32.const 3470268
                i32.load
                call $j895
                i32.load offset=12
                i32.const 0
                call $j899
                if  ;; label = @7
                  i32.const 1
                  local.set 2
                  br 6 (;@1;)
                end
                local.get 0
                i32.load offset=188
                local.get 1
                i32.const 3470268
                i32.load
                call $j895
                i32.load offset=12
                local.set 2
              end
              local.get 1
              i32.const 1
              i32.add
              local.set 1
              br 1 (;@4;)
            end
          end
          i32.const 3568632
          local.set 1
          br 1 (;@2;)
        end
        local.get 0
        local.get 0
        call $j19869
        local.set 0
        i32.const 3471676
        i32.load
        drop
        local.get 0
        i32.load offset=12
        br_if 1 (;@1;)
        i32.const 3568232
        local.set 1
      end
      local.get 1
      local.set 0
      i32.const 0
      local.set 2
      i32.const 3434152
      i32.load
      call $j871
      i32.const 600
      local.get 0
      i32.load
      i32.const 0
      call $j1117
    end
    local.get 3
    i32.const 16
    i32.add
    global.set 0
    local.get 2
    return)
