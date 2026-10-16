  (func $j38733 (type 0) (param i32 i32)
    (local i32 i32)
    i32.const 4511505
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3471272
      call $j870
      i32.const 3425048
      call $j870
      i32.const 4511505
      i32.const 1
      i32.store8
    end
    local.get 1
    i32.load offset=16
    i32.load offset=96
    i32.const 4
    call $j874
    call $j873
    local.set 2
    local.get 1
    i32.load offset=16
    i32.load offset=96
    i32.load offset=104
    local.set 3
    local.get 3
    i32.load offset=8
    local.get 1
    i32.load offset=16
    i32.load offset=96
    i32.load offset=104
    i32.load
    local.get 3
    local.get 2
    i32.const 0
    i32.const 0
    call $redir_viiiii
    local.get 0
    local.get 2
    i32.store offset=8
    i32.const 3425048
    i32.load
    call $j873
    local.set 2
    local.get 2
    i32.const 3471272
    i32.load
    call $j918
    local.get 0
    local.get 2
    i32.store offset=12
    local.get 1
    i32.load offset=16
    i32.load offset=96
    i32.const 1
    call $j874
    call $j873
    local.set 2
    local.get 1
    i32.load offset=16
    i32.load offset=96
    i32.load offset=108
    i32.load
    local.set 3
    local.get 1
    i32.load offset=16
    i32.load offset=96
    i32.load offset=108
    local.set 1
    local.get 1
    i32.load offset=8
    local.get 3
    local.get 1
    local.get 2
    i32.const 0
    i32.const 0
    call $redir_viiiii
    local.get 0
    local.get 2
    i32.store offset=16)