  (func $j6602 (type 1) (param i32 i32 i32)
    (local i32)
    i32.const 4507837
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3434312
      call $j870
      i32.const 3435416
      call $j870
      i32.const 4507837
      i32.const 1
      i32.store8
    end
    block  ;; label = @1
      local.get 1
      i32.eqz
      if  ;; label = @2
        local.get 0
        i32.load8_u offset=18
        br_if 1 (;@1;)
      end
      local.get 0
      i32.load offset=24
      local.get 1
      i32.const 0
      call $j907
      local.get 0
      local.get 1
      i32.const 1
      i32.xor
      i32.store8 offset=17
      local.get 0
      i32.load offset=32
      local.set 3
      i32.const 3435416
      i32.load
      call $j871
      i32.const 0
      local.set 2
      block  ;; label = @2
        local.get 3
        i32.const 0
        call $j982
        i32.eqz
        br_if 0 (;@2;)
        local.get 0
        i32.load offset=32
        i32.const 0
        call $j6545
        local.get 1
        i32.eq
        br_if 0 (;@2;)
        local.get 0
        i32.load offset=32
        local.get 1
        i32.const 0
        call $j2912
        i32.const 1
        local.set 2
      end
      local.get 0
      i32.load offset=36
      local.set 3
      i32.const 3435416
      i32.load
      call $j871
      block  ;; label = @2
        block  ;; label = @3
          block  ;; label = @4
            local.get 3
            i32.const 0
            call $j982
            i32.eqz
            br_if 0 (;@4;)
            local.get 0
            i32.load offset=36
            i32.const 0
            call $j6545
            local.get 1
            i32.eq
            br_if 0 (;@4;)
            local.get 0
            i32.load offset=36
            local.get 1
            i32.const 0
            call $j2912
            br 1 (;@3;)
          end
          local.get 2
          br_if 0 (;@3;)
          i32.const 0
          local.set 1
          br 1 (;@2;)
        end
        local.get 0
        local.get 1
        local.get 2
        call $j7955
        i32.const 0
        local.set 2
        loop  ;; label = @3
          local.get 0
          i32.load offset=40
          local.set 3
          local.get 2
          local.get 3
          i32.load offset=12
          i32.ge_s
          br_if 1 (;@2;)
          local.get 3
          local.get 2
          i32.const 2
          i32.shl
          i32.add
          i32.load offset=16
          local.get 1
          i32.const 0
          call $j2912
          local.get 2
          i32.const 1
          i32.add
          local.set 2
          br 0 (;@3;)
        end
        unreachable
      end
      i32.const 3434312
      i32.load
      call $j871
      i32.const 3434312
      i32.load
      i32.load offset=92
      i32.load8_u offset=4
      i32.eqz
      br_if 0 (;@1;)
      local.get 1
      i32.eqz
      br_if 0 (;@1;)
      local.get 0
      i32.load8_u offset=92
      i32.eqz
      br_if 0 (;@1;)
      local.get 0
      local.get 2
      call $j19265
    end)