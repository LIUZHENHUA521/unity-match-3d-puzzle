  (func $j8462 (type 29) (param i32 i32 f32 i32 i32) (result i32)
    (local f32 f32)
    global.get 0
    i32.const 16
    i32.sub
    local.set 4
    local.get 4
    global.set 0
    local.get 4
    local.get 2
    f32.store offset=8
    local.get 4
    local.get 1
    i32.store offset=12
    i32.const 4526032
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3466476
      call $j870
      i32.const 3466480
      call $j870
      i32.const 3434688
      call $j870
      i32.const 4526032
      i32.const 1
      i32.store8
    end
    local.get 4
    i32.const 0
    i32.store offset=4
    block  ;; label = @1
      block  ;; label = @2
        local.get 2
        f32.const 0x0p+0 (;=0;)
        f32.le
        i32.eqz
        if  ;; label = @3
          local.get 0
          i32.load offset=20
          i32.load8_u offset=61
          i32.eqz
          br_if 1 (;@2;)
          local.get 0
          i32.load offset=24
          br_if 1 (;@2;)
          local.get 0
          f32.load offset=28
          f32.const 0x0p+0 (;=0;)
          f32.ne
          br_if 1 (;@2;)
        end
        block  ;; label = @3
          block  ;; label = @4
            local.get 3
            i32.const 2
            i32.sub
            br_table 0 (;@4;) 1 (;@3;) 1 (;@3;) 0 (;@4;) 1 (;@3;)
          end
          local.get 1
          f32.const 0x0p+0 (;=0;)
          local.get 1
          call $j2185
        end
        local.get 0
        f32.const 0x1p+0 (;=1;)
        local.get 1
        call $j2832
        local.get 0
        local.get 1
        local.get 1
        call $j8463
        local.set 0
        br 1 (;@1;)
      end
      local.get 0
      local.get 3
      local.get 4
      i32.const 12
      i32.add
      local.get 4
      i32.const 8
      i32.add
      local.get 4
      i32.const 4
      i32.add
      local.get 1
      call $j44658
      local.get 0
      f32.const 0x1p+0 (;=1;)
      local.get 4
      f32.load offset=4
      local.get 1
      call $j6079
      local.get 4
      i32.load offset=12
      local.set 1
      local.get 0
      f32.load offset=28
      f32.const 0x0p+0 (;=0;)
      f32.eq
      if  ;; label = @2
        local.get 0
        local.get 1
        local.get 1
        call $j8463
        local.set 0
        br 1 (;@1;)
      end
      local.get 0
      local.get 1
      local.get 1
      call $j6972
      local.get 0
      local.get 4
      i32.load offset=12
      i32.store offset=52
      local.get 0
      local.get 0
      i32.load offset=56
      i32.const 1
      i32.add
      i32.store offset=56
      block  ;; label = @2
        block  ;; label = @3
          local.get 4
          i32.load offset=12
          local.set 1
          block  ;; label = @4
            local.get 1
            i32.load8_u offset=56
            i32.eqz
            br_if 0 (;@4;)
            local.get 1
            f32.load offset=36
            f32.const 0x1p+0 (;=1;)
            f32.ne
            br_if 0 (;@4;)
            local.get 1
            f32.load offset=28
            local.set 2
            local.get 2
            f32.const 0x1p+0 (;=1;)
            f32.eq
            br_if 1 (;@3;)
            local.get 1
            f32.load offset=40
            local.set 5
            local.get 4
            f32.load offset=8
            local.set 6
            i32.const 3434688
            i32.load
            call $j871
            f32.const 0x1p+0 (;=1;)
            local.get 2
            f32.sub
            f32.abs
            local.get 5
            local.get 6
            f32.mul
            f32.lt
            br_if 1 (;@3;)
            local.get 4
            i32.load offset=12
            local.set 1
          end
          local.get 1
          i32.const 1
          local.get 1
          call $j5374
          local.get 4
          f32.load offset=8
          local.set 2
          local.get 4
          i32.load offset=12
          f32.const 0x1p+0 (;=1;)
          local.get 2
          local.get 1
          call $j6079
          i32.const 3466476
          i32.load
          drop
          local.get 0
          i32.load offset=48
          i32.load offset=12
          i32.const 1
          i32.sub
          local.set 1
          loop  ;; label = @4
            local.get 1
            i32.const -1
            i32.le_s
            br_if 2 (;@2;)
            local.get 0
            i32.load offset=48
            local.get 1
            i32.const 3466480
            i32.load
            call $j895
            local.set 3
            local.get 3
            local.get 4
            i32.load offset=12
            i32.ne
            if  ;; label = @5
              local.get 3
              f32.const 0x0p+0 (;=0;)
              local.get 2
              local.get 1
              call $j6079
            end
            local.get 1
            i32.const 1
            i32.sub
            local.set 1
            br 0 (;@4;)
          end
          unreachable
        end
        local.get 0
        local.set 1
        local.get 0
        i32.load
        local.set 0
        local.get 0
        i32.load offset=476
        local.get 1
        local.get 0
        i32.load offset=480
        call $redir_vii
      end
      local.get 4
      i32.load offset=12
      local.set 0
    end
    local.get 4
    i32.const 16
    i32.add
    global.set 0
    local.get 0
    return)