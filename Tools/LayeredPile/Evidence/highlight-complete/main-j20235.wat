  (func $j20235 (type 14) (param i32 i32 i32 i32 i32 i32 i32)
    (local i32 f32 f32 f32 f32 f32 f32 f32 f32 f32 f32)
    global.get 0
    i32.const 288
    i32.sub
    local.set 0
    local.get 0
    global.set 0
    local.get 4
    f32.convert_i32_s
    local.set 8
    local.get 2
    local.get 2
    f32.load
    local.get 8
    f32.sub
    f32.store
    local.get 2
    f32.load offset=4
    local.get 8
    f32.sub
    local.set 9
    local.get 2
    local.get 9
    f32.store offset=4
    local.get 3
    f32.load
    local.get 8
    f32.add
    local.set 10
    local.get 3
    local.get 10
    f32.store
    local.get 3
    local.get 3
    f32.load offset=4
    local.get 8
    f32.add
    f32.store offset=4
    local.get 2
    f32.load offset=8
    local.set 8
    local.get 0
    local.get 2
    i32.load offset=8
    i32.store offset=88
    local.get 2
    local.set 7
    local.get 7
    i32.load
    local.set 4
    local.get 7
    i32.load offset=4
    local.set 6
    local.get 0
    local.get 4
    i32.store offset=80
    local.get 0
    local.get 6
    i32.store offset=84
    local.get 0
    i32.const 208
    i32.add
    local.get 1
    local.get 0
    i32.const 80
    i32.add
    i32.const 0
    call $j4366 ;; 
    local.get 7
    local.get 0
    i32.load offset=216
    i32.store offset=8
    local.get 0
    i32.load offset=208
    local.set 6
    local.get 0
    i32.load offset=212
    local.set 4
    local.get 6
    local.set 7
    local.get 2
    local.set 6
    local.get 6
    local.get 7
    i32.store
    local.get 6
    local.get 4
    i32.store offset=4
    local.get 0
    local.get 3
    i32.load offset=8
    i32.store offset=72
    local.get 3
    local.set 7
    local.get 7
    i32.load
    local.set 4
    local.get 7
    i32.load offset=4
    local.set 6
    local.get 0
    local.get 4
    i32.store offset=64
    local.get 0
    local.get 6
    i32.store offset=68
    local.get 0
    i32.const 208
    i32.add
    local.get 1
    local.get 0
    i32.const -64
    i32.sub
    i32.const 0
    call $j4366 ;; 
    local.get 7
    local.get 0
    i32.load offset=216
    i32.store offset=8
    local.get 0
    i32.load offset=208
    local.set 6
    local.get 0
    i32.load offset=212
    local.set 4
    local.get 6
    local.set 7
    local.get 3
    local.set 6
    local.get 6
    local.get 7
    i32.store
    local.get 6
    local.get 4
    i32.store offset=4
    local.get 0
    local.get 8
    f32.store offset=280
    local.get 0
    local.get 0
    i32.load offset=280
    i32.store offset=56
    local.get 0
    local.get 9
    f32.store offset=276
    local.get 0
    local.get 10
    f32.store offset=272
    local.get 0
    i32.load offset=272
    local.set 4
    local.get 0
    i32.load offset=276
    local.set 6
    local.get 0
    local.get 4
    i32.store offset=48
    local.get 0
    local.get 6
    i32.store offset=52
    local.get 0
    i32.const 208
    i32.add
    local.get 1
    local.get 0
    i32.const 48
    i32.add
    i32.const 0
    call $j4366 ;; 
    local.get 0
    f32.load offset=216
    local.set 8
    local.get 0
    f32.load offset=212
    local.set 9
    local.get 0
    f32.load offset=208
    local.set 10
    local.get 2
    f32.load offset=8
    local.set 14
    local.get 2
    f32.load offset=4
    local.set 15
    local.get 2
    f32.load
    local.set 16
    i32.const 4509457
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3434688
      call $j870 ;; 
      i32.const 4509457
      i32.const 1
      i32.store8
    end
    local.get 16
    local.get 10
    f32.sub
    local.set 11
    local.get 15
    local.get 9
    f32.sub
    local.set 12
    local.get 14
    local.get 8
    f32.sub
    local.set 13
    i32.const 3434688
    i32.load
    call $j871 ;; 
    local.get 11
    local.get 11
    f32.mul
    local.get 12
    local.get 12
    f32.mul
    f32.add
    local.get 13
    local.get 13
    f32.mul
    f32.add
    local.set 17
    local.get 3
    f32.load offset=8
    local.set 11
    local.get 3
    f32.load offset=4
    local.set 12
    local.get 3
    f32.load
    local.set 13
    i32.const 4509457
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3434688
      call $j870 ;; 
      i32.const 4509457
      i32.const 1
      i32.store8
    end
    local.get 13
    local.get 10
    f32.sub
    local.set 10
    local.get 12
    local.get 9
    f32.sub
    local.set 9
    local.get 11
    local.get 8
    f32.sub
    local.set 8
    i32.const 3434688
    i32.load
    call $j871 ;; 
    local.get 10
    local.get 10
    f32.mul
    local.get 9
    local.get 9
    f32.mul
    f32.add
    local.get 8
    local.get 8
    f32.mul
    f32.add
    local.set 8
    local.get 16
    local.get 13
    f32.add
    f32.const 0x1p-1 (;=0.5;)
    f32.mul
    local.set 9
    local.get 15
    local.get 12
    f32.add
    f32.const 0x1p-1 (;=0.5;)
    f32.mul
    local.set 10
    local.get 14
    local.get 11
    f32.add
    f32.const 0x1p-1 (;=0.5;)
    f32.mul
    local.set 14
    local.get 0
    i32.const 208
    i32.add
    local.get 1
    i32.const 0
    call $j944 ;; 
    i32.const 0
    call $j1610 ;; 
    local.get 0
    local.get 14
    f32.store offset=136
    local.get 0
    i32.load offset=216
    local.set 6
    local.get 0
    i32.load offset=220
    local.set 4
    local.get 6
    local.set 1
    local.get 0
    local.get 1
    i32.store offset=120
    local.get 0
    local.get 4
    i32.store offset=124
    local.get 0
    i32.const 1065353216
    i32.store offset=104
    local.get 0
    local.get 1
    i32.store offset=24
    local.get 0
    local.get 4
    i32.store offset=28
    local.get 0
    local.get 0
    i32.load offset=136
    i32.store offset=40
    local.get 0
    local.get 10
    f32.store offset=132
    local.get 0
    local.get 9
    f32.store offset=128
    local.get 0
    i32.load offset=208
    local.set 4
    local.get 0
    i32.load offset=212
    local.set 6
    local.get 4
    local.set 1
    local.get 0
    local.get 1
    i32.store offset=112
    local.get 0
    local.get 6
    i32.store offset=116
    local.get 0
    local.get 8
    f32.sqrt
    f32.store offset=100
    local.get 0
    local.get 17
    f32.sqrt
    f32.store offset=96
    local.get 0
    local.get 1
    i32.store offset=16
    local.get 0
    local.get 6
    i32.store offset=20
    local.get 0
    i32.load offset=128
    local.set 6
    local.get 0
    i32.load offset=132
    local.set 4
    local.get 0
    local.get 6
    i32.store offset=32
    local.get 0
    local.get 4
    i32.store offset=36
    local.get 0
    i32.const 1065353216
    i32.store offset=8
    local.get 0
    i32.load offset=96
    local.set 4
    local.get 0
    i32.load offset=100
    local.set 6
    local.get 0
    local.get 4
    i32.store
    local.get 0
    local.get 6
    i32.store offset=4
    local.get 0
    i32.const 144
    i32.add
    local.get 0
    i32.const 32
    i32.add
    local.get 0
    i32.const 16
    i32.add
    local.get 0
    i32.const 0
    call $j3053 ;; 
    local.get 0
    i32.const 208
    i32.add
    local.get 0
    i32.const 144
    i32.add
    i32.const 64
    call $j883 ;; 
    drop
    local.get 5
    local.get 0
    i32.const 208
    i32.add
    i32.const 64
    call $j883 ;; 
    drop
    local.get 0
    i32.const 288
    i32.add
    global.set 0)
