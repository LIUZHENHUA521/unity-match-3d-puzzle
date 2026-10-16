  (func $j19884 (type 2) (param i32 i32) (result i32)
    i32.const 4509315
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3432052
      call $j870
      i32.const 3434312
      call $j870
      i32.const 4509315
      i32.const 1
      i32.store8
    end
    block  ;; label = @1
      i32.const 3432052
      i32.load
      i32.load offset=92
      i32.load8_u offset=4
      i32.eqz
      br_if 0 (;@1;)
      i32.const 3432052
      i32.load
      i32.load offset=92
      i32.load8_u offset=12
      br_if 0 (;@1;)
      local.get 0
      i32.load8_u offset=60
      br_if 0 (;@1;)
      i32.const 3434312
      i32.load
      call $j871
      i32.const 3434312
      i32.load
      i32.load offset=92
      i32.load8_u offset=36
      br_if 0 (;@1;)
      i32.const 3434312
      i32.load
      call $j871
      i32.const 3434312
      i32.load
      i32.load offset=92
      i32.load8_u offset=5
      br_if 0 (;@1;)
      i32.const 3432052
      i32.load
      i32.load offset=92
      i32.load8_u offset=20
      i32.eqz
      return
    end
    i32.const 0
    return)