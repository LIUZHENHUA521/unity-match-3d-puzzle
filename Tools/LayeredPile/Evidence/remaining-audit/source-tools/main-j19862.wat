  (func $j19862 (type 1) (param i32 i32 i32)
    (local i32 i32 i32 f32 f32 f32 f32 f32)
    global.get 0
    i32.const 16
    i32.sub
    local.set 3
    local.get 3
    global.set 0
    i32.const 4509387
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3470264
      call $j870
      i32.const 3470268
      call $j870
      i32.const 3472464
      call $j870
      i32.const 3466200
      call $j870
      i32.const 3502396
      call $j870
      i32.const 4509387
      i32.const 1
      i32.store8
    end
    local.get 0
    i32.load offset=640
    local.get 1
    i32.const 3466200
    i32.load
    call $j895
    local.set 4
    i32.const 0
    local.set 2
    loop  ;; label = @1
      i32.const 3470264
      i32.load
      drop
      local.get 2
      local.get 4
      i32.load offset=12
      i32.lt_s
      if  ;; label = @2
        local.get 4
        local.get 2
        i32.const 3470268
        i32.load
        call $j895
        i32.const 0
        call $j944
        i32.const 0
        i32.const 0
        call $j1360
        drop
        i32.const 3470264
        i32.load
        drop
        local.get 0
        f32.load offset=652
        local.get 0
        f32.load offset=648
        f32.mul
        i32.const 1
        local.get 4
        i32.load offset=12
        local.get 2
        i32.sub
        i32.sub
        f32.convert_i32_s
        f32.mul
        local.set 6
        f32.const -0x1p+0 (;=-1;)
        f32.const 0x1p+0 (;=1;)
        local.get 2
        i32.const 1
        i32.and
        select
        local.get 0
        f32.load offset=656
        f32.mul
        local.set 8
        local.get 3
        local.get 0
        i32.load offset=636
        local.get 1
        i32.const 3472464
        i32.load
        call $j895
        i32.const 0
        call $j1028
        local.get 3
        f32.load offset=8
        local.set 9
        local.get 3
        f32.load offset=4
        local.set 7
        local.get 3
        f32.load
        local.set 10
        local.get 3
        local.get 0
        i32.load offset=636
        local.get 1
        i32.const 3472464
        i32.load
        call $j895
        i32.const 0
        call $j4551
        local.get 10
        local.get 3
        f32.load
        local.get 6
        f32.mul
        f32.add
        local.set 10
        local.get 7
        local.get 3
        f32.load offset=4
        local.get 6
        f32.mul
        f32.add
        local.set 7
        local.get 9
        local.get 3
        f32.load offset=8
        local.get 6
        f32.mul
        f32.add
        local.set 9
        i32.const 4524772
        i32.load8_u
        i32.eqz
        if  ;; label = @3
          i32.const 3439908
          call $j870
          i32.const 4524772
          i32.const 1
          i32.store8
        end
        i32.const 3439908
        i32.load
        i32.load offset=92
        local.set 5
        local.get 8
        f32.const 0x0p+0 (;=0;)
        local.get 2
        select
        local.set 6
        local.get 10
        local.get 5
        f32.load offset=60
        local.get 6
        f32.mul
        f32.add
        local.set 8
        local.get 7
        local.get 5
        f32.load offset=64
        local.get 6
        f32.mul
        f32.add
        local.set 7
        local.get 9
        local.get 5
        f32.load offset=68
        local.get 6
        f32.mul
        f32.add
        local.set 6
        local.get 4
        local.get 2
        i32.const 3470268
        i32.load
        call $j895
        i32.const 0
        call $j944
        local.get 8
        local.get 0
        f32.load offset=704
        i32.const 0
        i32.const 0
        call $j5721
        local.get 0
        i32.load offset=680
        i32.const 3502396
        i32.load
        call $j1183
        drop
        local.get 4
        local.get 2
        i32.const 3470268
        i32.load
        call $j895
        i32.const 0
        call $j944
        local.get 7
        local.get 0
        f32.load offset=704
        i32.const 0
        i32.const 0
        call $j9406
        local.get 0
        i32.load offset=688
        i32.const 3502396
        i32.load
        call $j1183
        drop
        local.get 4
        local.get 2
        i32.const 3470268
        i32.load
        call $j895
        i32.const 0
        call $j944
        local.get 6
        local.get 0
        f32.load offset=704
        i32.const 0
        i32.const 0
        call $j12483
        local.get 0
        i32.load offset=684
        i32.const 3502396
        i32.load
        call $j1183
        drop
        local.get 2
        i32.const 1
        i32.add
        local.set 2
        br 1 (;@1;)
      end
    end
    local.get 3
    i32.const 16
    i32.add
    global.set 0)
