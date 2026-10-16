  (func $j95669 (type 1) (param i32 i32 i32)
    i32.const 4508666
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3417424
      call $j870
      i32.const 3483376
      call $j870
      i32.const 3511096
      call $j870
      i32.const 3441648
      call $j870
      i32.const 4508666
      i32.const 1
      i32.store8
    end
    i32.const 3441648
    i32.load
    call $j873
    local.set 2
    local.get 2
    local.get 0
    i32.store offset=8
    local.get 1
    i32.const 1
    i32.const 3483376
    i32.load
    call $j1698
    local.set 0
    i32.const 3417424
    i32.load
    call $j873
    local.set 1
    local.get 1
    local.get 2
    i32.const 3511096
    i32.load
    i32.const 0
    call $j931
    local.get 0
    local.get 1
    i32.const 0
    call $j1933)