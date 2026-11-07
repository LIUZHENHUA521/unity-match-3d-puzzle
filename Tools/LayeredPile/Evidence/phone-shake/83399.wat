  (func $j83399 (type 1) (param i32 i32 i32)
    (local i32 i32 i32 i32 f32 f32 f32 f32 f32 f32 f32 f32 f32)
    global.get 0
    i32.const 32
    i32.sub
    local.set 2
    local.get 2
    global.set 0
    i32.const 4509409
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3430720
      call $j870 ;; 
      i32.const 3432052
      call $j870 ;; 
      i32.const 3470264
      call $j870 ;; 
      i32.const 3470268
      call $j870 ;; 
      i32.const 3438764
      call $j870 ;; 
      i32.const 3566124
      call $j870 ;; 
      i32.const 3521756
      call $j870 ;; 
      i32.const 3565692
      call $j870 ;; 
      i32.const 4509409
      i32.const 1
      i32.store8
    end
    local.get 2
    i32.const 0
    i32.store offset=28
    local.get 0
    i32.const 0
    call $j1744 ;; Time.get_unscaledTime
    local.get 0
    f32.load offset=776
    f32.sub
    f32.store offset=780
    local.get 0
    i32.const 0
    call $j1744 ;; Time.get_unscaledTime
    f32.store offset=776
    local.get 0
    f32.load offset=780
    local.set 7
    f32.const 0x1.0e5604p-5 (;=0.033;)
    local.get 7
    f32.const 0x1p+0 (;=1;)
    f32.min
    local.get 7
    f32.const 0x1.0e5604p-5 (;=0.033;)
    f32.lt
    select
    f32.const 0x1.4p+2 (;=5;)
    f32.mul
    local.set 7
    local.get 0
    local.get 7
    f32.store offset=784
    local.get 7
    f32.const 0x1.8p+2 (;=6;)
    f32.mul
    local.set 7
    local.get 0
    local.get 7
    f32.store offset=784
    local.get 0
    local.get 7
    i32.const 0
    call $j17292 ;; Time.get_fixedDeltaTime
    f32.div
    f32.const 0x1.47ae14p-6 (;=0.02;)
    f32.mul
    f32.store offset=784
    block  ;; label = @1
      i32.const 3432052
      i32.load
      i32.load offset=92
      i32.load8_u offset=4
      i32.eqz
      br_if 0 (;@1;)
      i32.const 3432052
      i32.load
      i32.load offset=92
      i32.load8_u offset=12
      br_if 0 (;@1;)
      i32.const 3438764
      i32.load
      call $j871 ;; 
      i32.const 3438764
      i32.load
      i32.load offset=92
      i32.load8_u offset=36
      i32.eqz
      br_if 0 (;@1;)
      local.get 1
      f64.load offset=16
      f32.demote_f64
      local.set 7
      local.get 1
      f64.load offset=24
      f32.demote_f64
      local.set 9
      local.get 1
      f64.load offset=8
      f32.demote_f64
      local.set 10
      i32.const 4523251
      i32.load8_u
      i32.eqz
      if  ;; label = @2
        i32.const 3434688
        call $j870 ;; 
        i32.const 4523251
        i32.const 1
        i32.store8
      end
      i32.const 3434688
      i32.load
      call $j871 ;; 
      local.get 0
      f32.load offset=796
      local.get 10
      local.get 10
      f32.mul
      local.get 9
      local.get 9
      f32.mul
      f32.add
      local.get 7
      local.get 7
      f32.mul
      f32.add
      f32.sqrt
      f32.gt
      br_if 0 (;@1;)
      i32.const 4523251
      i32.load8_u
      i32.eqz
      if  ;; label = @2
        i32.const 3434688
        call $j870 ;; 
        i32.const 4523251
        i32.const 1
        i32.store8
      end
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
      local.get 7
      local.get 7
      f32.mul
      f32.add
      f32.sqrt
      local.set 8
      block  ;; label = @2
        local.get 8
        f32.const 0x1.4f8b58p-17 (;=1e-05;)
        f32.gt
        if  ;; label = @3
          local.get 7
          local.get 8
          f32.div
          local.set 13
          local.get 10
          local.get 8
          f32.div
          local.set 14
          local.get 9
          local.get 8
          f32.div
          local.set 15
          br 1 (;@2;)
        end
        i32.const 4525920
        i32.load8_u
        i32.eqz
        if  ;; label = @3
          i32.const 3439908
          call $j870 ;; 
          i32.const 4525920
          i32.const 1
          i32.store8
        end
        i32.const 3439908
        i32.load
        i32.load offset=92
        local.set 1
        local.get 1
        f32.load offset=8
        local.set 13
        local.get 1
        f32.load
        local.set 14
        local.get 1
        f32.load offset=4
        local.set 15
      end
      i32.const 4523252
      i32.load8_u
      i32.eqz
      if  ;; label = @2
        i32.const 3434688
        call $j870 ;; 
        i32.const 4523252
        i32.const 1
        i32.store8
      end
      local.get 0
      i32.const 784
      i32.add
      local.set 3
      local.get 0
      i32.const 780
      i32.add
      local.set 4
      i32.const 3434688
      i32.load
      call $j871 ;; 
      local.get 0
      f32.load offset=800
      local.set 11
      local.get 10
      local.get 10
      f32.mul
      local.get 9
      local.get 9
      f32.mul
      f32.add
      local.get 7
      local.get 7
      f32.mul
      f32.add
      f32.sqrt
      local.get 0
      f32.load offset=796
      f32.sub
      local.set 8
      f32.const 0x0p+0 (;=0;)
      local.get 11
      local.get 8
      local.get 8
      local.get 11
      f32.gt
      select
      local.get 8
      f32.const 0x0p+0 (;=0;)
      f32.lt
      select
      local.set 8
      i32.const 0
      local.set 1
      loop  ;; label = @2
        i32.const 3470264
        i32.load
        drop
        local.get 1
        local.get 0
        i32.load offset=192
        i32.load offset=12
        i32.lt_s
        if  ;; label = @3
          local.get 0
          i32.load offset=192
          local.get 1
          i32.const 3470268
          i32.load
          call $j895 ;; 
          i32.const 0
          call $j36729 ;; ObjectUnit.CanShake
          if  ;; label = @4
            local.get 0
            i32.load offset=192
            local.get 1
            i32.const 3470268
            i32.load
            call $j895 ;; 
            local.set 5
            local.get 0
            f32.load offset=804
            local.set 11
            local.get 0
            f32.load offset=784
            local.set 12
            local.get 2
            local.get 13
            local.get 8
            f32.mul
            local.get 11
            f32.mul
            local.get 12
            f32.mul
            f32.store offset=24
            local.get 2
            local.get 2
            i32.load offset=24
            i32.store offset=8
            local.get 2
            local.get 15
            local.get 8
            f32.mul
            local.get 11
            f32.mul
            local.get 12
            f32.mul
            f32.store offset=20
            local.get 2
            local.get 14
            local.get 8
            f32.mul
            local.get 11
            f32.mul
            local.get 12
            f32.mul
            f32.store offset=16
            local.get 2
            i32.load offset=20
            local.set 6
            local.get 2
            local.get 2
            i32.load offset=16
            i32.store
            local.get 2
            local.get 6
            i32.store offset=4
            local.get 5
            local.get 2
            i32.const 0
            call $j36730 ;; ObjectUnit.AddForce
          end
          local.get 1
          i32.const 1
          i32.add
          local.set 1
          br 1 (;@2;)
        end
      end
      local.get 0
      i32.load offset=792
      local.set 1
      local.get 1
      local.get 0
      i32.load offset=788
      i32.lt_s
      if  ;; label = @2
        local.get 0
        local.get 1
        i32.const 1
        i32.add
        i32.store offset=792
        i32.const 4523252
        i32.load8_u
        i32.eqz
        if  ;; label = @3
          i32.const 3434688
          call $j870 ;; 
          i32.const 4523252
          i32.const 1
          i32.store8
        end
        i32.const 3434688
        i32.load
        call $j871 ;; 
        local.get 2
        local.get 10
        local.get 10
        f32.mul
        local.get 9
        local.get 9
        f32.mul
        f32.add
        local.get 7
        local.get 7
        f32.mul
        f32.add
        f32.sqrt
        f32.store offset=28
        local.get 2
        i32.const 28
        i32.add
        i32.const 0
        call $j1472 ;; Single.ToString
        local.set 1
        local.get 3
        i32.const 0
        call $j1472 ;; Single.ToString
        local.set 3
        local.get 4
        i32.const 0
        call $j1472 ;; Single.ToString
        local.set 4
        i32.const 3566124
        i32.load
        local.get 1
        i32.const 3565692
        i32.load
        local.get 3
        i32.const 3521756
        i32.load
        local.get 4
        i32.const 0
        call $j1051 ;; GameManager.StringAppend
        local.set 1
        i32.const 3430720
        i32.load
        call $j871 ;; 
        local.get 1
        i32.const 0
        call $j1049 ;; Debug.Log
      end
      local.get 0
      i32.const 0
      i32.store offset=812
      local.get 0
      f32.load offset=780
      i32.const 0
      call $j33405 ;; SportManager.AddCal
    end
    local.get 2
    i32.const 32
    i32.add
    global.set 0)
