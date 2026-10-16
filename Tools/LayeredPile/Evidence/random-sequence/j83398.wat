  (func $j83398 (type 0) (param i32 i32)
    (local i32 i32 f32)
    global.get 0
    i32.const 208
    i32.sub
    local.set 1
    local.get 1
    global.set 0
    i32.const 4509312
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3428500
      call $j870
      i32.const 3428552
      call $j870
      i32.const 3429624
      call $j870
      i32.const 3430720
      call $j870
      i32.const 3431312
      call $j870
      i32.const 3431796
      call $j870
      i32.const 3432392
      call $j870
      i32.const 3434308
      call $j870
      i32.const 3493380
      call $j870
      i32.const 3434540
      call $j870
      i32.const 3435240
      call $j870
      i32.const 3435256
      call $j870
      i32.const 3437300
      call $j870
      i32.const 3437348
      call $j870
      i32.const 3438324
      call $j870
      i32.const 3438336
      call $j870
      i32.const 3439744
      call $j870
      i32.const 3440576
      call $j870
      i32.const 3565824
      call $j870
      i32.const 3569664
      call $j870
      i32.const 3565884
      call $j870
      i32.const 3566988
      call $j870
      i32.const 3565800
      call $j870
      i32.const 3556524
      call $j870
      i32.const 3569876
      call $j870
      i32.const 3565600
      call $j870
      i32.const 3567648
      call $j870
      i32.const 3567348
      call $j870
      i32.const 3520228
      call $j870
      i32.const 3565604
      call $j870
      i32.const 3538012
      call $j870
      i32.const 3532036
      call $j870
      i32.const 3569284
      call $j870
      i32.const 4509312
      i32.const 1
      i32.store8
    end
    local.get 1
    i32.const 0
    i32.store offset=204
    local.get 1
    i32.const 0
    i32.store offset=200
    local.get 1
    i32.const 0
    i32.store offset=196
    local.get 1
    i32.const 0
    i32.store offset=192
    local.get 1
    i32.const 0
    i32.store offset=188
    local.get 1
    i32.const 0
    i32.store offset=184
    local.get 1
    i32.const 0
    i32.store offset=180
    local.get 1
    i32.const 0
    i32.store offset=176
    local.get 1
    i32.const 0
    i32.store offset=172
    local.get 1
    i32.const 0
    i32.store offset=168
    local.get 1
    i32.const 0
    i32.store offset=164
    local.get 0
    i32.load offset=164
    i32.load offset=16
    i32.const 0
    call $j6027
    local.set 4
    loop  ;; label = @1
      local.get 0
      i32.load offset=164
      local.set 3
      local.get 3
      i32.load offset=12
      local.get 2
      i32.gt_s
      if  ;; label = @2
        local.get 3
        local.get 2
        i32.const 2
        i32.shl
        i32.add
        i32.load offset=16
        local.get 4
        i32.const 3429624
        i32.load
        i32.load offset=92
        f32.load offset=4
        f32.mul
        i32.const 0
        call $j3793
        local.get 2
        i32.const 1
        i32.add
        local.set 2
        br 1 (;@1;)
      end
    end
    local.get 0
    local.get 2
    call $j38508
    i32.const 3428500
    i32.load
    call $j871
    i32.const 0
    call $j22255
    i32.const 1
    i32.eq
    if  ;; label = @1
      local.get 0
      i32.const 3538012
      i32.load
      i32.const 0
      call $j1535
      drop
    end
    local.get 0
    local.get 2
    call $j38500
    i32.const 3435240
    i32.load
    call $j871
    local.get 2
    call $j2392
    i32.eqz
    if  ;; label = @1
      i32.const 3428500
      i32.load
      call $j871
      local.get 1
      i32.const 0
      call $j14393
      i32.store offset=204
      i32.const 3434540
      i32.load
      local.set 3
      local.get 1
      i32.const 152
      i32.add
      local.set 2
      local.get 2
      i32.const -1
      i32.store offset=4
      local.get 2
      local.get 3
      i32.store
      local.get 2
      local.get 1
      i32.load offset=204
      i32.store offset=8
      local.get 2
      i32.const 0
      call $j956
      local.set 2
      local.get 2
      local.set 3
      i32.const 3520228
      i32.load
      local.set 2
      i32.const 3567648
      i32.load
      local.get 3
      local.get 2
      local.get 2
      local.get 2
      local.get 2
      i32.const 0
      call $j1051
      local.set 2
      i32.const 3430720
      i32.load
      call $j871
      local.get 2
      i32.const 0
      call $j1049
      local.get 1
      i32.const 0
      call $j10585
      i32.store offset=200
      i32.const 3434308
      i32.load
      local.set 3
      local.get 1
      i32.const 136
      i32.add
      local.set 2
      local.get 2
      i32.const -1
      i32.store offset=4
      local.get 2
      local.get 3
      i32.store
      local.get 2
      local.get 1
      i32.load offset=200
      i32.store offset=8
      local.get 2
      i32.const 0
      call $j956
      local.set 2
      local.get 2
      local.set 3
      i32.const 3520228
      i32.load
      local.set 2
      i32.const 3569876
      i32.load
      local.get 3
      local.get 2
      local.get 2
      local.get 2
      local.get 2
      i32.const 0
      call $j1051
      i32.const 0
      call $j1049
      local.get 1
      i32.const 0
      call $j44897
      i32.store offset=196
      i32.const 3437348
      i32.load
      local.set 3
      local.get 1
      i32.const 120
      i32.add
      local.set 2
      local.get 2
      i32.const -1
      i32.store offset=4
      local.get 2
      local.get 3
      i32.store
      local.get 2
      local.get 1
      i32.load offset=196
      i32.store offset=8
      local.get 2
      i32.const 0
      call $j956
      local.set 2
      local.get 2
      local.set 3
      i32.const 3520228
      i32.load
      local.set 2
      i32.const 3569664
      i32.load
      local.get 3
      local.get 2
      local.get 2
      local.get 2
      local.get 2
      i32.const 0
      call $j1051
      i32.const 0
      call $j1049
      local.get 1
      i32.const 0
      call $j22256
      i32.store offset=192
      i32.const 3431312
      i32.load
      local.set 3
      local.get 1
      i32.const 104
      i32.add
      local.set 2
      local.get 2
      i32.const -1
      i32.store offset=4
      local.get 2
      local.get 3
      i32.store
      local.get 2
      local.get 1
      i32.load offset=192
      i32.store offset=8
      local.get 2
      i32.const 0
      call $j956
      local.set 2
      local.get 2
      local.set 3
      i32.const 3520228
      i32.load
      local.set 2
      i32.const 3565600
      i32.load
      local.get 3
      local.get 2
      local.get 2
      local.get 2
      local.get 2
      i32.const 0
      call $j1051
      i32.const 0
      call $j1049
      local.get 1
      i32.const 0
      call $j10582
      i32.store offset=188
      i32.const 3438324
      i32.load
      local.set 3
      local.get 1
      i32.const 88
      i32.add
      local.set 2
      local.get 2
      i32.const -1
      i32.store offset=4
      local.get 2
      local.get 3
      i32.store
      local.get 2
      local.get 1
      i32.load offset=188
      i32.store offset=8
      local.get 2
      i32.const 0
      call $j956
      local.set 2
      local.get 2
      local.set 3
      i32.const 3520228
      i32.load
      local.set 2
      i32.const 3567348
      i32.load
      local.get 3
      local.get 2
      local.get 2
      local.get 2
      local.get 2
      i32.const 0
      call $j1051
      i32.const 0
      call $j1049
      local.get 1
      i32.const 0
      call $j14394
      i32.store offset=184
      i32.const 3437300
      i32.load
      local.set 3
      local.get 1
      i32.const 72
      i32.add
      local.set 2
      local.get 2
      i32.const -1
      i32.store offset=4
      local.get 2
      local.get 3
      i32.store
      local.get 2
      local.get 1
      i32.load offset=184
      i32.store offset=8
      local.get 2
      i32.const 0
      call $j956
      local.set 2
      local.get 2
      local.set 3
      i32.const 3520228
      i32.load
      local.set 2
      i32.const 3565884
      i32.load
      local.get 3
      local.get 2
      local.get 2
      local.get 2
      local.get 2
      i32.const 0
      call $j1051
      i32.const 0
      call $j1049
      local.get 1
      i32.const 0
      call $j10584
      i32.store offset=180
      i32.const 3431796
      i32.load
      local.set 3
      local.get 1
      i32.const 56
      i32.add
      local.set 2
      local.get 2
      i32.const -1
      i32.store offset=4
      local.get 2
      local.get 3
      i32.store
      local.get 2
      local.get 1
      i32.load offset=180
      i32.store offset=8
      local.get 2
      i32.const 0
      call $j956
      local.set 2
      local.get 2
      local.set 3
      i32.const 3520228
      i32.load
      local.set 2
      i32.const 3565604
      i32.load
      local.get 3
      local.get 2
      local.get 2
      local.get 2
      local.get 2
      i32.const 0
      call $j1051
      i32.const 0
      call $j1049
      local.get 1
      i32.const 0
      call $j14395
      i32.store offset=176
      local.get 1
      i32.const 176
      i32.add
      i32.const 0
      call $j973
      local.set 2
      local.get 2
      local.set 3
      i32.const 3520228
      i32.load
      local.set 2
      i32.const 3565800
      i32.load
      local.get 3
      local.get 2
      local.get 2
      local.get 2
      local.get 2
      i32.const 0
      call $j1051
      i32.const 0
      call $j1049
      local.get 1
      i32.const 0
      call $j22255
      i32.store offset=172
      i32.const 3438336
      i32.load
      local.set 3
      local.get 1
      i32.const 40
      i32.add
      local.set 2
      local.get 2
      i32.const -1
      i32.store offset=4
      local.get 2
      local.get 3
      i32.store
      local.get 2
      local.get 1
      i32.load offset=172
      i32.store offset=8
      local.get 2
      i32.const 0
      call $j956
      local.set 2
      local.get 2
      local.set 3
      i32.const 3520228
      i32.load
      local.set 2
      i32.const 3565824
      i32.load
      local.get 3
      local.get 2
      local.get 2
      local.get 2
      local.get 2
      i32.const 0
      call $j1051
      i32.const 0
      call $j1049
      local.get 1
      i32.const 0
      call $j10583
      i32.store offset=168
      i32.const 3439744
      i32.load
      local.set 3
      local.get 1
      i32.const 24
      i32.add
      local.set 2
      local.get 2
      i32.const -1
      i32.store offset=4
      local.get 2
      local.get 3
      i32.store
      local.get 2
      local.get 1
      i32.load offset=168
      i32.store offset=8
      local.get 2
      i32.const 0
      call $j956
      local.set 2
      local.get 2
      local.set 3
      i32.const 3520228
      i32.load
      local.set 2
      i32.const 3569284
      i32.load
      local.get 3
      local.get 2
      local.get 2
      local.get 2
      local.get 2
      i32.const 0
      call $j1051
      i32.const 0
      call $j1049
      local.get 1
      i32.const 0
      call $j4845
      i32.store offset=164
      i32.const 3432392
      i32.load
      local.set 3
      local.get 1
      i32.const 8
      i32.add
      local.set 2
      local.get 2
      i32.const -1
      i32.store offset=4
      local.get 2
      local.get 3
      i32.store
      local.get 2
      local.get 1
      i32.load offset=164
      i32.store offset=8
      local.get 2
      i32.const 0
      call $j956
      local.set 2
      local.get 2
      local.set 3
      i32.const 3520228
      i32.load
      local.set 2
      i32.const 3566988
      i32.load
      local.get 3
      local.get 2
      local.get 2
      local.get 2
      local.get 2
      i32.const 0
      call $j1051
      i32.const 0
      call $j1049
    end
    i32.const 3435256
    i32.load
    call $j871
    i32.const 3435256
    i32.load
    i32.load offset=92
    i32.load offset=16
    i32.const 3556524
    i32.load
    i32.const 0
    call $j899
    if  ;; label = @1
      local.get 0
      local.get 0
      f32.load offset=808
      f32.store offset=804
    end
    local.get 0
    local.get 2
    call $j19880
    i32.const 3435240
    i32.load
    call $j871
    i32.const 3435240
    i32.load
    i32.load offset=92
    i32.load
    local.get 2
    call $j19377
    if  ;; label = @1
      i32.const 3428552
      i32.load
      call $j873
      local.set 2
      local.get 2
      local.get 0
      i32.const 3493380
      i32.load
      i32.const 0
      call $j988
      i32.const 3440576
      i32.load
      call $j871
      i32.const 3532036
      i32.load
      local.get 2
      i32.const 0
      call $j28242
    end
    local.get 1
    i32.const 208
    i32.add
    global.set 0)