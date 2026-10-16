  (func $j38487 (type 1) (param i32 i32 i32)
    (local i32)
    i32.const 4509385
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3470244
      call $j870
      i32.const 3470256
      call $j870
      i32.const 3470264
      call $j870
      i32.const 3466196
      call $j870
      i32.const 3466200
      call $j870
      i32.const 4509385
      i32.const 1
      i32.store8
    end
    i32.const 0
    local.set 2
    loop  ;; label = @1
      block  ;; label = @2
        i32.const 3466196
        i32.load
        drop
        local.get 2
        local.get 0
        i32.load offset=640
        i32.load offset=12
        i32.ge_s
        br_if 0 (;@2;)
        local.get 0
        i32.load offset=640
        local.get 2
        i32.const 3466200
        i32.load
        call $j895
        local.set 3
        i32.const 3470264
        i32.load
        drop
        block  ;; label = @3
          local.get 3
          i32.load offset=12
          i32.const 1
          i32.lt_s
          br_if 0 (;@3;)
          local.get 0
          i32.load offset=640
          local.get 2
          i32.const 3466200
          i32.load
          call $j895
          local.get 1
          i32.const 3470244
          i32.load
          call $j1235
          i32.eqz
          br_if 0 (;@3;)
          local.get 0
          i32.load offset=640
          local.get 2
          i32.const 3466200
          i32.load
          call $j895
          local.get 1
          i32.const 3470256
          i32.load
          call $j1342
          drop
          local.get 0
          local.get 2
          local.get 2
          call $j19862
          br 1 (;@2;)
        end
        local.get 2
        i32.const 1
        i32.add
        local.set 2
        br 1 (;@1;)
      end
    end
    local.get 0
    local.get 2
    call $j19868
    i32.eqz
    if  ;; label = @1
      local.get 0
      i32.const 0
      i32.const 1
      local.get 2
      call $j9876
    end)
