  (func $j9352 (type 0) (param i32 i32)
    i32.const 4508434
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3435416
      call $j870
      i32.const 3437580
      call $j870
      i32.const 3538028
      call $j870
      i32.const 4508434
      i32.const 1
      i32.store8
    end
    i32.const 3437580
    i32.load
    call $j871
    i32.const 3437580
    i32.load
    i32.load offset=92
    local.get 0
    i32.store offset=28
    i32.const 3437580
    i32.load
    i32.load offset=92
    i32.load
    local.set 1
    i32.const 3435416
    i32.load
    call $j871
    block  ;; label = @1
      local.get 1
      i32.const 0
      i32.const 0
      call $j924
      br_if 0 (;@1;)
      block  ;; label = @2
        block  ;; label = @3
          local.get 0
          i32.const 1
          i32.sub
          br_table 0 (;@3;) 1 (;@2;) 2 (;@1;)
        end
        i32.const 3437580
        i32.load
        call $j871
        i32.const 3437580
        i32.load
        i32.load offset=92
        i32.load offset=32
        local.set 0
        i32.const 3435416
        i32.load
        call $j871
        local.get 0
        i32.const 0
        i32.const 0
        call $j915
        i32.eqz
        br_if 1 (;@1;)
        i32.const 3437580
        i32.load
        call $j871
        i32.const 3437580
        i32.load
        i32.load offset=92
        i32.load
        i32.load offset=36
        i32.const 3437580
        i32.load
        i32.load offset=92
        i32.load
        f32.load offset=40
        i32.const 3437580
        i32.load
        i32.load offset=92
        f32.load offset=40
        f32.mul
        i32.const 0
        call $j8410
        i32.const 3437580
        i32.load
        i32.load offset=92
        i32.load
        i32.load offset=36
        i32.const 3437580
        i32.load
        i32.load offset=92
        i32.load offset=32
        i32.const 0
        call $j10465
        i32.const 3437580
        i32.load
        i32.load offset=92
        i32.load
        i32.load offset=36
        i32.const 0
        call $j10466
        i32.const 3437580
        i32.load
        i32.load offset=92
        i32.load
        i32.const 3538028
        i32.load
        i32.const 0
        call $j1446
        i32.const 3437580
        i32.load
        i32.load offset=92
        i32.load
        i32.const 3538028
        i32.load
        i32.const 0
        call $j1535
        drop
        return
      end
      i32.const 3437580
      i32.load
      call $j871
      i32.const 3437580
      i32.load
      i32.load offset=92
      i32.load offset=36
      local.set 0
      i32.const 3435416
      i32.load
      call $j871
      local.get 0
      i32.const 0
      i32.const 0
      call $j915
      i32.eqz
      br_if 0 (;@1;)
      i32.const 3437580
      i32.load
      call $j871
      i32.const 3437580
      i32.load
      i32.load offset=92
      i32.load
      i32.load offset=36
      i32.const 3437580
      i32.load
      i32.load offset=92
      i32.load
      f32.load offset=44
      i32.const 3437580
      i32.load
      i32.load offset=92
      f32.load offset=40
      f32.mul
      i32.const 0
      call $j8410
      i32.const 3437580
      i32.load
      i32.load offset=92
      i32.load
      i32.load offset=36
      i32.const 3437580
      i32.load
      i32.load offset=92
      i32.load offset=36
      i32.const 0
      call $j10465
      i32.const 3437580
      i32.load
      i32.load offset=92
      i32.load
      i32.load offset=36
      i32.const 0
      call $j10466
    end)
