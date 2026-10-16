  (func $j19860 (type 1) (param i32 i32 i32)
    i32.const 4509384
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3470236
      call $j870
      i32.const 3466196
      call $j870
      i32.const 3466200
      call $j870
      i32.const 4509384
      i32.const 1
      i32.store8
    end
    local.get 0
    local.get 0
    call $j19868
    i32.eqz
    if  ;; label = @1
      local.get 0
      i32.const 1
      i32.const 1
      local.get 0
      call $j9876
    end
    local.get 0
    i32.load offset=640
    local.get 0
    i32.load offset=644
    i32.const 3466200
    i32.load
    call $j895
    local.get 1
    i32.const 3470236
    i32.load
    call $j903
    local.get 0
    local.get 0
    i32.load offset=644
    local.get 0
    call $j19862
    local.get 0
    i32.load offset=644
    i32.const 1
    i32.add
    local.set 2
    local.get 0
    local.get 2
    i32.store offset=644
    i32.const 3466196
    i32.load
    drop
    local.get 0
    local.get 2
    local.get 0
    i32.load offset=640
    i32.load offset=12
    call $i32s_rem
    i32.store offset=644
    local.get 0
    local.get 1
    i32.const 0
    call $j944
    local.get 0
    call $j38515)
