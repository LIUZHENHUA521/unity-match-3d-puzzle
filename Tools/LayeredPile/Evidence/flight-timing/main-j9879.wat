  (func $j9879 (type 7) (param i32 i32 i32 i32)
    (local i32 i32 i32 i32 i32 i32 i32 i32 i32 f32)
    global.get 0
    i32.const 48
    i32.sub
    local.set 3
    local.get 3
    global.set 0
    i32.const 4509371
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3470264
      call $j870 ;; 
      i32.const 3470268
      call $j870 ;; 
      i32.const 3472464
      call $j870 ;; 
      i32.const 3502372
      call $j870 ;; 
      i32.const 3502396
      call $j870 ;; 
      i32.const 3502480
      call $j870 ;; 
      i32.const 3502500
      call $j870 ;; 
      i32.const 4509371
      i32.const 1
      i32.store8
    end
    local.get 1
    i32.const 1
    i32.eq
    local.set 4
    local.get 0
    i32.const 184
    i32.const 200
    local.get 4
    select
    i32.add
    i32.load
    local.set 9
    local.get 0
    i32.const 188
    i32.const 204
    local.get 4
    select
    i32.add
    i32.load
    local.set 8
    i32.const 0
    local.set 4
    loop  ;; label = @1
      i32.const 3470264
      i32.load
      drop
      local.get 4
      local.get 8
      i32.load offset=12
      i32.lt_s
      if  ;; label = @2
        local.get 8
        local.get 4
        i32.const 3470268
        i32.load
        call $j895 ;; 
        i32.const 0
        call $j944 ;; 
        local.set 6
        local.get 3
        i32.const 32
        i32.add
        local.get 9
        local.get 4
        i32.const 3472464
        i32.load
        call $j895 ;; 
        i32.const 0
        call $j1028 ;; 
        local.get 6
        local.get 3
        f32.load offset=32
        local.get 0
        f32.load offset=584
        i32.const 0
        i32.const 0
        call $j5721 ;; 
        i32.const 1
        i32.const 3502500
        i32.load
        call $j1281 ;; 
        local.get 0
        i32.load offset=588
        i32.const 3502396
        i32.load
        call $j1183 ;; 
        drop
        local.get 8
        local.get 4
        i32.const 3470268
        i32.load
        call $j895 ;; 
        i32.const 0
        call $j944 ;; 
        local.set 6
        local.get 3
        i32.const 32
        i32.add
        local.get 9
        local.get 4
        i32.const 3472464
        i32.load
        call $j895 ;; 
        i32.const 0
        call $j1028 ;; 
        local.get 6
        local.get 3
        f32.load offset=36
        local.get 0
        f32.load offset=584
        i32.const 0
        i32.const 0
        call $j9406 ;; 
        i32.const 1
        i32.const 3502500
        i32.load
        call $j1281 ;; 
        local.get 0
        i32.load offset=592
        i32.const 3502396
        i32.load
        call $j1183 ;; 
        drop
        local.get 8
        local.get 4
        i32.const 3470268
        i32.load
        call $j895 ;; 
        i32.const 0
        call $j944 ;; 
        local.set 6
        local.get 3
        i32.const 32
        i32.add
        local.get 9
        local.get 4
        i32.const 3472464
        i32.load
        call $j895 ;; 
        i32.const 0
        call $j1028 ;; 
        local.get 6
        local.get 3
        f32.load offset=40
        local.get 0
        f32.load offset=584
        i32.const 0
        i32.const 0
        call $j12483 ;; 
        i32.const 1
        i32.const 3502500
        i32.load
        call $j1281 ;; 
        local.get 0
        i32.load offset=588
        i32.const 3502396
        i32.load
        call $j1183 ;; 
        drop
        local.get 8
        local.get 4
        i32.const 3470268
        i32.load
        call $j895 ;; 
        i32.const 0
        call $j944 ;; 
        local.set 6
        local.get 3
        i32.const 32
        i32.add
        local.get 9
        local.get 4
        i32.const 3472464
        i32.load
        call $j895 ;; 
        i32.const 0
        call $j1610 ;; 
        local.get 3
        i32.load offset=40
        local.set 5
        local.get 3
        i32.load offset=44
        local.set 7
        local.get 5
        local.set 10
        local.get 7
        local.set 12
        local.get 3
        local.get 10
        i32.store offset=24
        local.get 3
        local.get 7
        i32.store offset=28
        local.get 3
        i32.load offset=32
        local.set 7
        local.get 3
        i32.load offset=36
        local.set 5
        local.get 7
        local.set 11
        local.get 5
        local.set 7
        local.get 3
        local.get 11
        i32.store offset=16
        local.get 3
        local.get 5
        i32.store offset=20
        local.get 0
        f32.load offset=584
        local.set 13
        local.get 12
        local.set 5
        local.get 3
        local.get 10
        i32.store offset=8
        local.get 3
        local.get 5
        i32.store offset=12
        local.get 7
        local.set 5
        local.get 3
        local.get 11
        i32.store
        local.get 3
        local.get 5
        i32.store offset=4
        local.get 6
        local.get 3
        local.get 13
        i32.const 0
        call $j9405 ;; 
        i32.const 1
        i32.const 3502480
        i32.load
        call $j1281 ;; 
        local.get 0
        i32.load offset=596
        i32.const 3502372
        i32.load
        call $j1183 ;; 
        drop
        local.get 4
        i32.const 1
        i32.add
        local.set 4
        br 1 (;@1;)
      end
    end
    local.get 2
    if  ;; label = @1
      local.get 0
      local.get 0
      local.get 1
      local.get 0
      f32.load offset=584
      local.get 3
      call $j19875 ;; LevelManager.DelayToCheckContent
      i32.const 0
      call $j1135 ;; 
      drop
    end
    local.get 3
    i32.const 48
    i32.add
    global.set 0)
