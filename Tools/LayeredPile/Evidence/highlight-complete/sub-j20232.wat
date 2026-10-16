  (func $j20232 (type 5) (param i32 i32 i32) (result i32)
    (local i32 i32)
    i32.const 4505055
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3435416
      i32.const 870
      call_indirect (type 0)
      i32.const 4505055
      i32.const 1
      i32.store8
    end
    i32.const 0
    local.set 2
    loop  ;; label = @1
      block  ;; label = @2
        local.get 2
        local.get 0
        i32.load offset=420
        i32.lt_s
        local.set 3
        local.get 3
        i32.eqz
        br_if 0 (;@2;)
        local.get 0
        i32.load offset=416
        local.get 2
        i32.const 164
        i32.mul
        i32.add
        i32.const 16
        i32.add
        i32.load offset=4
        local.set 4
        i32.const 3435416
        i32.load
        i32.const 871
        call_indirect (type 0)
        local.get 4
        local.get 1
        i32.const 0
        i32.const 924
        call_indirect (type 5)
        br_if 0 (;@2;)
        local.get 2
        i32.const 1
        i32.add
        local.set 2
        br 1 (;@1;)
      end
    end
    local.get 3
    return)
