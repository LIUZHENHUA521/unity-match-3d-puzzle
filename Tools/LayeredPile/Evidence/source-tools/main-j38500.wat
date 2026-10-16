  (func $j38500 (type 0) (param i32 i32)
    (local i32 i32 i32 f32 f32 f32 f32 f32 f32)
    global.get 0
    i32.const 16
    i32.sub
    local.set 1
    local.get 1
    global.set 0
    i32.const 4509382
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3466192
      call $j870
      i32.const 3470232
      call $j870
      i32.const 3472460
      call $j870
      i32.const 3472464
      call $j870
      i32.const 3424832
      call $j870
      i32.const 4509382
      i32.const 1
      i32.store8
    end
    i32.const 0
    local.get 0
    i32.load offset=636
    local.get 0
    i32.load offset=632
    i32.const 0
    call $j13224
    loop  ;; label = @1
      i32.const 3472460
      i32.load
      drop
      local.get 2
      local.get 0
      i32.load offset=636
      i32.load offset=12
      i32.lt_s
      if  ;; label = @2
        local.get 0
        i32.load offset=640
        local.set 4
        i32.const 3424832
        i32.load
        call $j873
        local.set 3
        local.get 3
        i32.const 3470232
        i32.load
        call $j918
        local.get 4
        local.get 3
        i32.const 3466192
        i32.load
        call $j903
        local.get 2
        i32.const 1
        i32.add
        local.set 2
        br 1 (;@1;)
      end
    end
    local.get 1
    local.get 0
    i32.load offset=636
    i32.const 0
    i32.const 3472464
    i32.load
    call $j895
    i32.const 0
    call $j1028
    local.get 1
    f32.load offset=8
    local.set 5
    local.get 1
    f32.load offset=4
    local.set 6
    local.get 1
    f32.load
    local.set 7
    local.get 1
    local.get 0
    i32.load offset=636
    i32.const 1
    i32.const 3472464
    i32.load
    call $j895
    i32.const 0
    call $j1028
    local.get 1
    f32.load offset=8
    local.set 8
    local.get 1
    f32.load offset=4
    local.set 9
    local.get 1
    f32.load
    local.set 10
    i32.const 4509457
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3434688
      call $j870
      i32.const 4509457
      i32.const 1
      i32.store8
    end
    local.get 7
    local.get 10
    f32.sub
    local.set 7
    local.get 6
    local.get 9
    f32.sub
    local.set 6
    local.get 5
    local.get 8
    f32.sub
    local.set 5
    i32.const 3434688
    i32.load
    call $j871
    local.get 0
    local.get 7
    local.get 7
    f32.mul
    local.get 6
    local.get 6
    f32.mul
    f32.add
    local.get 5
    local.get 5
    f32.mul
    f32.add
    f32.sqrt
    f32.store offset=652
    local.get 0
    i32.const 0
    i32.const 0
    local.get 2
    call $j9876
    local.get 1
    i32.const 16
    i32.add
    global.set 0)
