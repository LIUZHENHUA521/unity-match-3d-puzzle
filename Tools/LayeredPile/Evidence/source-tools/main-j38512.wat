  (func $j38512 (type 2) (param i32 i32) (result i32)
    (local i32)
    i32.const 4509402
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3471620
      call $j870
      i32.const 3471632
      call $j870
      i32.const 3471608
      call $j870
      i32.const 3470264
      call $j870
      i32.const 3470268
      call $j870
      i32.const 3425124
      call $j870
      i32.const 4509402
      i32.const 1
      i32.store8
    end
    i32.const 3425124
    i32.load
    call $j873
    local.set 2
    local.get 2
    i32.const 3471608
    i32.load
    call $j918
    i32.const 3470264
    i32.load
    drop
    local.get 0
    i32.load offset=192
    i32.load offset=12
    local.set 1
    loop  ;; label = @1
      local.get 1
      i32.const 1
      i32.sub
      local.set 1
      local.get 1
      i32.const -1
      i32.gt_s
      if  ;; label = @2
        local.get 0
        i32.load offset=192
        local.get 1
        i32.const 3470268
        i32.load
        call $j895
        i32.load8_u offset=16
        i32.eqz
        br_if 1 (;@1;)
        local.get 2
        local.get 0
        i32.load offset=192
        local.get 1
        i32.const 3470268
        i32.load
        call $j895
        i32.load offset=12
        i32.const 3471632
        i32.load
        call $j1235
        br_if 1 (;@1;)
        local.get 2
        local.get 0
        i32.load offset=192
        local.get 1
        i32.const 3470268
        i32.load
        call $j895
        i32.load offset=12
        i32.const 3471620
        i32.load
        call $j903
        br 1 (;@1;)
      end
    end
    local.get 2
    return)
