  (func $j38499 (type 0) (param i32 i32)
    i32.const 4509383
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3470240
      call $j870
      i32.const 3466196
      call $j870
      i32.const 3466200
      call $j870
      i32.const 4509383
      i32.const 1
      i32.store8
    end
    i32.const 0
    local.set 1
    loop  ;; label = @1
      i32.const 3466196
      i32.load
      drop
      local.get 1
      local.get 0
      i32.load offset=640
      i32.load offset=12
      i32.lt_s
      if  ;; label = @2
        local.get 0
        i32.load offset=640
        local.get 1
        i32.const 3466200
        i32.load
        call $j895
        i32.const 3470240
        i32.load
        call $j974
        local.get 1
        i32.const 1
        i32.add
        local.set 1
        br 1 (;@1;)
      end
    end
    local.get 0
    i32.const 0
    i32.const 0
    local.get 1
    call $j9876)
