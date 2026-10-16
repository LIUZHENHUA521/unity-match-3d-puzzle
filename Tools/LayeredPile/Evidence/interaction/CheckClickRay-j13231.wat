  (func $j13231 (type 1) (param i32 i32 i32)
    (local i32 i32 i32 i32 i32)
    global.get 0
    i32.const 112
    i32.sub
    local.set 2
    local.get 2
    global.set 0
    i32.const 4509334
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3491012
      call $j870
      i32.const 3434312
      call $j870
      i32.const 3435416
      call $j870
      i32.const 3440008
      call $j870
      i32.const 3539932
      call $j870
      i32.const 4509334
      i32.const 1
      i32.store8
    end
    local.get 1
    i32.const 1
    i32.eq
    local.set 5
    local.get 0
    i32.const 444
    i32.add
    local.get 0
    i32.const 452
    i32.add
    local.get 5
    select
    local.set 3
    local.get 3
    i32.load
    local.set 4
    local.get 3
    i32.load offset=4
    local.set 3
    local.get 0
    i32.const 252
    i32.const 316
    local.get 5
    select
    i32.add
    i32.load
    local.set 6
    local.get 0
    i32.load offset=144
    local.set 7
    i32.const 3434312
    i32.load
    call $j871
    local.get 2
    call $j1923
    if  ;; label = @1
      local.get 0
      i32.load offset=164
      local.get 1
      i32.const 1
      i32.sub
      i32.const 2
      i32.shl
      i32.add
      i32.load offset=16
      local.set 7
    end
    local.get 2
    i32.const 0
    i32.store offset=56
    local.get 2
    i32.const 0
    i32.store offset=40
    local.get 2
    local.get 4
    i32.store offset=48
    local.get 2
    local.get 3
    i32.store offset=52
    local.get 2
    local.get 4
    i32.store offset=32
    local.get 2
    local.get 3
    i32.store offset=36
    local.get 2
    i32.const -64
    i32.sub
    local.get 7
    local.get 2
    i32.const 32
    i32.add
    i32.const 0
    call $j2414
    local.get 2
    i32.load offset=80
    local.set 3
    local.get 2
    i32.load offset=84
    local.set 4
    local.get 2
    local.get 3
    i32.store offset=104
    local.get 2
    local.get 4
    i32.store offset=108
    local.get 2
    i32.load offset=72
    local.set 4
    local.get 2
    i32.load offset=76
    local.set 3
    local.get 2
    local.get 4
    i32.store offset=96
    local.get 2
    local.get 3
    i32.store offset=100
    local.get 2
    i32.load offset=64
    local.set 3
    local.get 2
    i32.load offset=68
    local.set 4
    local.get 2
    local.get 3
    i32.store offset=88
    local.get 2
    local.get 4
    i32.store offset=92
    i32.const 3539932
    i32.load
    i32.const 0
    call $j4708
    local.set 5
    local.get 2
    i32.load offset=104
    local.set 4
    local.get 2
    i32.load offset=108
    local.set 3
    local.get 2
    local.get 4
    i32.store offset=24
    local.get 2
    local.get 3
    i32.store offset=28
    local.get 2
    i32.load offset=96
    local.set 3
    local.get 2
    i32.load offset=100
    local.set 4
    local.get 2
    local.get 3
    i32.store offset=16
    local.get 2
    local.get 4
    i32.store offset=20
    local.get 2
    i32.load offset=88
    local.set 4
    local.get 2
    i32.load offset=92
    local.set 3
    local.get 2
    local.get 4
    i32.store offset=8
    local.get 2
    local.get 3
    i32.store offset=12
    local.get 0
    i32.const 256
    i32.add
    local.set 3
    block  ;; label = @1
      local.get 2
      i32.const 8
      i32.add
      local.get 3
      f32.const 0x1.9p+6 (;=100;)
      i32.const 1
      local.get 5
      i32.shl
      i32.const 0
      call $j7924
      if  ;; label = @2
        i32.const 3435416
        i32.load
        call $j871
        local.get 6
        i32.const 0
        i32.const 0
        call $j915
        if  ;; label = @3
          local.get 6
          i32.const 0
          call $j925
          local.set 4
          local.get 3
          i32.const 0
          call $j9602
          i32.const 0
          call $j925
          local.set 5
          i32.const 3435416
          i32.load
          call $j871
          local.get 4
          local.get 5
          i32.const 0
          call $j924
          if  ;; label = @4
            local.get 0
            i32.const 0
            local.get 2
            call $j19859
            br 3 (;@1;)
          end
          local.get 0
          local.get 2
          call $j38504
          i32.eqz
          br_if 2 (;@1;)
        end
        local.get 3
        i32.const 0
        call $j6564
        i32.const 0
        call $j925
        i32.const 3491012
        i32.load
        call $j1083
        local.set 5
        block  ;; label = @3
          i32.const 0
          i32.const 0
          call $j2547
          i32.eqz
          br_if 0 (;@3;)
          i32.const 3434312
          i32.load
          call $j871
          local.get 2
          call $j1923
          br_if 0 (;@3;)
          i32.const 3434312
          i32.load
          call $j871
          local.get 2
          call $j5871
          if  ;; label = @4
            local.get 0
            i32.load offset=628
            i32.load offset=56
            i32.load8_u offset=45
            i32.eqz
            br_if 1 (;@3;)
          end
          local.get 5
          i32.const 0
          call $j1073
          local.set 4
          i32.const 3440008
          i32.load
          call $j871
          local.get 4
          i32.const 0
          call $j7474
        end
        i32.const 0
        local.set 4
        block  ;; label = @3
          local.get 5
          i32.load8_u offset=16
          i32.eqz
          br_if 0 (;@3;)
          local.get 5
          i32.load8_u offset=17
          br_if 0 (;@3;)
          local.get 5
          i32.load8_u offset=92
          i32.const 0
          i32.ne
          local.set 4
        end
        local.get 4
        i32.eqz
        br_if 1 (;@1;)
        local.get 0
        i32.const 0
        local.get 2
        call $j19859
        i32.const 3435416
        i32.load
        call $j871
        local.get 6
        i32.const 0
        i32.const 0
        call $j915
        if  ;; label = @3
          local.get 6
          i32.const 0
          i32.const 0
          i32.const 0
          call $j6603
        end
        local.get 5
        i32.const 1
        local.get 7
        i32.const 0
        call $j944
        i32.const 0
        call $j6603
        local.get 0
        i32.load offset=112
        local.get 1
        i32.const 1
        i32.sub
        i32.const 2
        i32.shl
        i32.add
        i32.load offset=16
        local.get 5
        i32.const 0
        call $j944
        i32.const 0
        call $j8126
        local.get 0
        i32.load offset=112
        local.get 1
        i32.const 1
        i32.sub
        i32.const 2
        i32.shl
        i32.add
        i32.load offset=16
        i32.const 1
        i32.const 0
        call $j2810
        local.get 1
        i32.const 1
        i32.eq
        if  ;; label = @3
          local.get 0
          i32.const 252
          i32.add
          local.get 5
          i32.store
          br 2 (;@1;)
        end
        local.get 0
        i32.const 316
        i32.add
        local.get 5
        i32.store
        br 1 (;@1;)
      end
      local.get 0
      local.get 1
      local.get 2
      call $j13226
    end
    local.get 2
    i32.const 112
    i32.add
    global.set 0)
