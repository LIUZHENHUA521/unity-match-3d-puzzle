  (func $j2594 (type 37) (param i32 f32 i32) (result i32)
    (local i32 f32)
    i32.const -1
    local.set 2
    block  ;; label = @1
      local.get 0
      i32.load
      local.set 3
      local.get 0
      f32.load
      local.set 4
      local.get 4
      local.get 1
      f32.lt
      br_if 0 (;@1;)
      i32.const 1
      local.set 2
      local.get 1
      local.get 4
      f32.lt
      br_if 0 (;@1;)
      i32.const 0
      local.set 2
      local.get 1
      local.get 4
      f32.eq
      br_if 0 (;@1;)
      i32.const 4526225
      i32.load8_u
      i32.eqz
      if  ;; label = @2
        i32.const 3429344
        call $j870
        i32.const 4526225
        i32.const 1
        i32.store8
      end
      i32.const 3429344
      i32.load
      call $j871
      i32.const 1
      local.set 2
      local.get 3
      i32.const 2147483647
      i32.and
      i32.const 2139095041
      i32.lt_u
      br_if 0 (;@1;)
      i32.const 4526225
      i32.load8_u
      i32.eqz
      if  ;; label = @2
        i32.const 3429344
        call $j870
        i32.const 4526225
        i32.const 1
        i32.store8
      end
      i32.const 3429344
      i32.load
      call $j871
      i32.const -1
      i32.const 0
      local.get 1
      i32.reinterpret_f32
      i32.const 2147483647
      i32.and
      i32.const 2139095041
      i32.lt_u
      select
      return
    end
    local.get 2
    return)