  (func $j65003 (type 2) (param i32 i32) (result i32)
    (local i32 i32 f32 f32)
    i32.const 0
    local.set 1
    i32.const 4507840
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3435416
      call $j870
      i32.const 3440336
      call $j870
      i32.const 4507840
      i32.const 1
      i32.store8
    end
    local.get 0
    i32.load offset=16
    local.set 2
    local.get 0
    local.set 3
    block  ;; label = @1
      block  ;; label = @2
        block  ;; label = @3
          block  ;; label = @4
            block  ;; label = @5
              block  ;; label = @6
                block  ;; label = @7
                  block  ;; label = @8
                    local.get 0
                    i32.load offset=8
                    br_table 0 (;@8;) 1 (;@7;) 2 (;@6;) 3 (;@5;) 4 (;@4;) 7 (;@1;)
                  end
                  local.get 0
                  i32.const -1
                  i32.store offset=8
                  br 4 (;@3;)
                end
                local.get 0
                i32.const -1
                i32.store offset=8
                local.get 2
                i32.load offset=44
                local.set 1
                i32.const 3435416
                i32.load
                call $j871
                local.get 1
                i32.const 0
                call $j982
                i32.eqz
                br_if 3 (;@3;)
                local.get 2
                i32.const 0
                i32.const 0
                f32.const 0x1.99999ap-3 (;=0.2;)
                local.get 0
                call $j7956
                local.set 4
                f32.const 0x1p+0 (;=1;)
                f32.const 0x1.8p+1 (;=3;)
                i32.const 0
                call $j1345
                local.set 5
                i32.const 3440336
                i32.load
                call $j873
                local.set 1
                local.get 1
                local.get 4
                local.get 5
                f32.mul
                f32.store offset=8
                local.get 0
                local.get 1
                i32.store offset=12
                i32.const 2
                local.set 0
                br 4 (;@2;)
              end
              local.get 0
              i32.const -1
              i32.store offset=8
              local.get 2
              i32.const 0
              i32.const 1
              f32.const 0x1.99999ap-2 (;=0.4;)
              local.get 0
              call $j7956
              drop
              br 2 (;@3;)
            end
            local.get 0
            i32.const -1
            i32.store offset=8
            local.get 2
            i32.load offset=44
            local.set 1
            i32.const 3435416
            i32.load
            call $j871
            local.get 1
            i32.const 0
            call $j982
            i32.eqz
            br_if 1 (;@3;)
            local.get 2
            i32.const 1
            i32.const 0
            f32.const 0x1.99999ap-3 (;=0.2;)
            local.get 0
            call $j7956
            local.set 4
            f32.const 0x1p+0 (;=1;)
            f32.const 0x1.8p+1 (;=3;)
            i32.const 0
            call $j1345
            local.set 5
            i32.const 3440336
            i32.load
            call $j873
            local.set 1
            local.get 1
            local.get 4
            local.get 5
            f32.mul
            f32.store offset=8
            local.get 0
            local.get 1
            i32.store offset=12
            i32.const 4
            local.set 0
            br 2 (;@2;)
          end
          local.get 0
          i32.const -1
          i32.store offset=8
          local.get 2
          i32.const 1
          i32.const 1
          f32.const 0x1.99999ap-2 (;=0.4;)
          local.get 0
          call $j7956
          drop
        end
        local.get 2
        i32.load8_u offset=16
        i32.eqz
        if  ;; label = @3
          f32.const 0x1p+0 (;=1;)
          f32.const 0x1.4p+2 (;=5;)
          i32.const 0
          call $j1345
          local.set 4
          i32.const 3440336
          i32.load
          call $j873
          local.set 1
          local.get 1
          local.get 4
          f32.store offset=8
          local.get 0
          local.get 1
          i32.store offset=12
          i32.const 1
          local.set 0
          br 1 (;@2;)
        end
        f32.const 0x1.4p+2 (;=5;)
        f32.const 0x1.ep+3 (;=15;)
        i32.const 0
        call $j1345
        local.set 4
        i32.const 3440336
        i32.load
        call $j873
        local.set 1
        local.get 1
        local.get 4
        f32.store offset=8
        local.get 0
        local.get 1
        i32.store offset=12
        i32.const 3
        local.set 0
      end
      local.get 3
      local.get 0
      i32.store offset=8
      i32.const 1
      local.set 1
    end
    local.get 1
    return)