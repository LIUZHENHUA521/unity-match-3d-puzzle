  (func $j33491 (type 1) (param i32 i32 i32)
    i32.const 4508429
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3435416
      call $j870
      i32.const 3437580
      call $j870
      i32.const 4508429
      i32.const 1
      i32.store8
    end
    i32.const 3437580
    i32.load
    call $j871
    block  ;; label = @1
      i32.const 3437580
      i32.load
      i32.load offset=92
      i32.load offset=20
      i32.load8_u offset=8
      i32.eqz
      br_if 0 (;@1;)
      i32.const 3437580
      i32.load
      call $j871
      i32.const 3437580
      i32.load
      i32.load offset=92
      i32.load
      local.set 2
      i32.const 3435416
      i32.load
      call $j871
      local.get 2
      i32.const 0
      i32.const 0
      call $j924
      br_if 0 (;@1;)
      i32.const 3437580
      i32.load
      call $j871
      i32.const 3437580
      i32.load
      i32.load offset=92
      i32.load
      i32.load offset=60
      local.set 2
      local.get 1
      if  ;; label = @2
        local.get 2
        i32.const 0
        call $j43723
        i32.const 3437580
        i32.load
        i32.load offset=92
        i32.load
        i32.load offset=60
        local.set 2
      end
      local.get 2
      local.get 0
      i32.const 0
      call $j5355
    end)
