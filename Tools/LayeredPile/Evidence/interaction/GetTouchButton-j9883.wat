  (func $j9883 (type 5) (param i32 i32 i32) (result i32)
    (local i32 i32 i32 i32 i32 f32 f32)
    global.get 0
    i32.const 144
    i32.sub
    local.set 2
    local.get 2
    global.set 0
    i32.const 4509333
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3434312
      call $j870
      i32.const 4509333
      i32.const 1
      i32.store8
    end
    local.get 2
    i32.const 72
    i32.add
    i32.const 0
    i32.const 68
    call $j890
    drop
    block  ;; label = @1
      block  ;; label = @2
        block  ;; label = @3
          i32.const 3434312
          i32.load
          call $j871
          local.get 2
          call $j1923
          i32.eqz
          br_if 0 (;@3;)
          i32.const 0
          call $j3737
          i32.const 1
          i32.lt_s
          br_if 2 (;@1;)
          local.get 1
          i32.const 1
          i32.ne
          local.set 5
          local.get 1
          i32.const 2
          i32.ne
          local.set 6
          i32.const 0
          local.set 1
          loop  ;; label = @4
            i32.const 0
            call $j3737
            local.get 1
            i32.le_s
            br_if 3 (;@1;)
            local.get 2
            local.get 1
            i32.const 0
            call $j3530
            local.get 2
            i32.const 72
            i32.add
            local.get 2
            i32.const 68
            call $j883
            drop
            local.get 2
            i32.load offset=76
            local.set 3
            local.get 2
            i32.load offset=80
            local.set 4
            local.get 2
            local.get 3
            i32.store
            local.get 2
            local.get 4
            i32.store offset=4
            local.get 2
            f32.load offset=4
            local.set 8
            i32.const 0
            call $j1572
            f32.convert_i32_s
            f32.const 0x1p-1 (;=0.5;)
            f32.mul
            local.set 9
            block  ;; label = @5
              local.get 5
              br_if 0 (;@5;)
              local.get 8
              local.get 9
              f32.lt
              i32.eqz
              br_if 0 (;@5;)
              local.get 0
              i32.const 1
              i32.store8 offset=440
              local.get 2
              local.get 1
              i32.const 0
              call $j3530
              local.get 2
              i32.const 72
              i32.add
              local.get 2
              i32.const 68
              call $j883
              drop
              local.get 2
              i32.load offset=76
              local.set 4
              local.get 2
              i32.load offset=80
              local.set 3
              local.get 2
              local.get 4
              i32.store
              local.get 2
              local.get 3
              i32.store offset=4
              local.get 0
              i32.const 444
              i32.add
              local.set 0
              br 3 (;@2;)
            end
            local.get 2
            local.get 1
            i32.const 0
            call $j3530
            local.get 2
            i32.const 72
            i32.add
            local.get 2
            i32.const 68
            call $j883
            drop
            local.get 2
            i32.load offset=76
            local.set 3
            local.get 2
            i32.load offset=80
            local.set 4
            local.get 2
            local.get 3
            i32.store
            local.get 2
            local.get 4
            i32.store offset=4
            local.get 2
            f32.load offset=4
            local.set 8
            i32.const 0
            call $j1572
            f32.convert_i32_s
            f32.const 0x1p-1 (;=0.5;)
            f32.mul
            local.set 9
            block  ;; label = @5
              local.get 6
              br_if 0 (;@5;)
              local.get 8
              local.get 9
              f32.ge
              i32.eqz
              br_if 0 (;@5;)
              local.get 0
              i32.const 1
              i32.store8 offset=441
              local.get 2
              local.get 1
              i32.const 0
              call $j3530
              local.get 2
              i32.const 72
              i32.add
              local.get 2
              i32.const 68
              call $j883
              drop
              local.get 2
              i32.load offset=76
              local.set 4
              local.get 2
              i32.load offset=80
              local.set 3
              local.get 2
              local.get 4
              i32.store
              local.get 2
              local.get 3
              i32.store offset=4
              local.get 0
              i32.const 452
              i32.add
              local.set 0
              br 3 (;@2;)
            end
            local.get 1
            i32.const 1
            i32.add
            local.set 1
            br 0 (;@4;)
          end
          unreachable
        end
        i32.const 3434312
        i32.load
        call $j871
        local.get 2
        call $j1923
        local.set 6
        i32.const 0
        i32.const 0
        call $j3738
        local.set 5
        local.get 6
        if  ;; label = @3
          local.get 5
          i32.eqz
          br_if 2 (;@1;)
          local.get 2
          i32.const 0
          call $j1860
          local.get 2
          f32.load offset=4
          local.set 8
          i32.const 0
          call $j1572
          f32.convert_i32_s
          f32.const 0x1p-1 (;=0.5;)
          f32.mul
          local.set 9
          block  ;; label = @4
            local.get 1
            i32.const 1
            i32.ne
            br_if 0 (;@4;)
            local.get 8
            local.get 9
            f32.lt
            i32.eqz
            br_if 0 (;@4;)
            local.get 0
            i32.const 1
            i32.store8 offset=440
            local.get 2
            i32.const 0
            call $j1860
            local.get 0
            i32.const 444
            i32.add
            local.set 0
            br 2 (;@2;)
          end
          local.get 2
          i32.const 0
          call $j1860
          local.get 2
          f32.load offset=4
          i32.const 0
          call $j1572
          f32.convert_i32_s
          f32.const 0x1p-1 (;=0.5;)
          f32.mul
          f32.ge
          i32.eqz
          br_if 2 (;@1;)
          local.get 1
          i32.const 2
          i32.ne
          br_if 2 (;@1;)
          local.get 0
          i32.const 1
          i32.store8 offset=441
          local.get 2
          i32.const 0
          call $j1860
          local.get 0
          i32.const 452
          i32.add
          local.set 0
          br 1 (;@2;)
        end
        local.get 5
        i32.eqz
        br_if 1 (;@1;)
        local.get 0
        i32.const 1
        i32.store8 offset=440
        local.get 2
        i32.const 0
        call $j1860
        local.get 0
        i32.const 444
        i32.add
        local.set 0
      end
      local.get 2
      i32.load
      local.set 3
      local.get 2
      i32.load offset=4
      local.set 4
      local.get 3
      local.set 1
      local.get 0
      local.set 3
      local.get 3
      local.get 1
      i32.store
      local.get 3
      local.get 4
      i32.store offset=4
      i32.const 1
      local.set 7
    end
    local.get 2
    i32.const 144
    i32.add
    global.set 0
    local.get 7
    return)