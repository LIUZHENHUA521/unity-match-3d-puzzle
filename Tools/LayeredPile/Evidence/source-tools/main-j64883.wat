  (func $j64883 (type 2) (param i32 i32) (result i32)
    (local i32 i32 i32 i32 i32 i32 i32 i32 i32 i32 f32 f32 f32)
    global.get 0
    i32.const 48
    i32.sub
    local.set 1
    local.get 1
    global.set 0
    i32.const 4507444
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3484204
      call $j870
      i32.const 3434152
      call $j870
      i32.const 3438972
      call $j870
      i32.const 3502328
      call $j870
      i32.const 3502388
      call $j870
      i32.const 3440336
      call $j870
      i32.const 4507444
      i32.const 1
      i32.store8
    end
    local.get 0
    i32.load offset=16
    local.set 4
    block  ;; label = @1
      block  ;; label = @2
        block  ;; label = @3
          local.get 0
          i32.load offset=8
          br_table 0 (;@3;) 1 (;@2;) 2 (;@1;)
        end
        local.get 0
        i32.const -1
        i32.store offset=8
        local.get 4
        i32.load offset=12
        local.set 6
        i32.const 4513575
        i32.load8_u
        i32.eqz
        if  ;; label = @3
          i32.const 3439888
          call $j870
          i32.const 4513575
          i32.const 1
          i32.store8
        end
        i32.const 3439888
        i32.load
        i32.load offset=92
        local.set 2
        local.get 2
        f32.load offset=20
        local.set 12
        local.get 2
        f32.load offset=16
        local.set 13
        i32.const 3434152
        i32.load
        call $j871
        local.get 1
        i32.const 40
        i32.add
        i32.const 0
        call $j3727
        i32.const 0
        call $j1243
        local.get 1
        local.set 9
        local.get 13
        local.set 14
        local.get 1
        f32.load offset=44
        f32.const 0x1p-1 (;=0.5;)
        f32.mul
        f32.const 0x1p+8 (;=256;)
        f32.add
        local.set 13
        i32.const 0
        local.set 2
        local.get 14
        local.get 13
        f32.mul
        i32.reinterpret_f32
        local.set 10
        local.get 2
        local.set 11
        i32.const 0
        local.set 2
        local.get 12
        local.get 13
        f32.mul
        i32.reinterpret_f32
        local.set 3
        i32.const 32
        local.set 7
        local.get 7
        i32.const 31
        i32.and
        local.set 8
        i32.const 32
        local.get 7
        i32.const 63
        i32.and
        i32.le_u
        if  ;; label = @3
          local.get 3
          local.get 8
          i32.shl
          local.set 5
          i32.const 0
          local.set 7
        else
          i32.const 1
          local.get 8
          i32.shl
          i32.const 1
          i32.sub
          local.get 3
          i32.const 32
          local.get 8
          i32.sub
          i32.shr_u
          i32.and
          local.get 2
          local.get 8
          i32.shl
          i32.or
          local.set 5
          local.get 3
          local.get 8
          i32.shl
          local.set 7
        end
        local.get 5
        local.set 3
        local.get 11
        local.set 5
        local.get 10
        local.set 2
        local.get 5
        local.get 3
        i32.or
        local.set 3
        local.get 2
        local.get 7
        i32.or
        local.set 5
        local.get 9
        local.set 2
        local.get 2
        local.get 5
        i32.store offset=16
        local.get 2
        local.get 3
        i32.store offset=20
        local.get 1
        local.set 2
        local.get 2
        local.get 5
        i32.store offset=32
        local.get 2
        local.get 3
        i32.store offset=36
        local.get 6
        local.get 2
        i32.const 16
        i32.add
        i32.const 0
        call $j1207
        local.get 4
        i32.load offset=12
        local.set 6
        local.get 4
        i32.load offset=44
        f32.load offset=44
        local.set 12
        local.get 4
        i32.load offset=56
        local.set 3
        local.get 4
        i32.load offset=60
        local.set 2
        local.get 3
        local.set 5
        local.get 1
        local.set 3
        local.get 3
        local.get 5
        i32.store offset=8
        local.get 3
        local.get 2
        i32.store offset=12
        local.get 3
        local.get 5
        i32.store offset=24
        local.get 3
        local.get 2
        i32.store offset=28
        local.get 6
        local.get 3
        i32.const 8
        i32.add
        local.get 12
        i32.const 0
        i32.const 0
        call $j5304
        local.get 4
        i32.load offset=24
        i32.const 3502388
        i32.load
        call $j1183
        local.set 6
        i32.const 3438972
        i32.load
        call $j873
        local.set 2
        local.get 2
        local.get 4
        i32.const 3484204
        i32.load
        i32.const 0
        call $j1486
        local.get 6
        local.get 2
        i32.const 3502328
        i32.load
        call $j3015
        drop
        local.get 4
        i32.load offset=44
        f32.load offset=44
        local.set 12
        i32.const 3440336
        i32.load
        call $j873
        local.set 4
        local.get 4
        local.get 12
        f32.store offset=8
        local.get 0
        local.get 4
        i32.store offset=12
        i32.const 1
        local.set 6
        local.get 0
        i32.const 1
        i32.store offset=8
        br 1 (;@1;)
      end
      local.get 0
      i32.const -1
      i32.store offset=8
      local.get 4
      i32.load offset=12
      i32.const 0
      i32.const 0
      call $j1360
      drop
      local.get 4
      i32.load offset=12
      i32.const 0
      call $j925
      i32.const 0
      i32.const 0
      call $j907
    end
    local.get 1
    i32.const 48
    i32.add
    global.set 0
    local.get 6
    return)
