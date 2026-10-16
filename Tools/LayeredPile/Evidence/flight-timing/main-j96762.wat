  (func $j96762 (type 4) (param i32) (result i32)
    (local i32 i32)
    global.get 0
    i32.const 16
    i32.sub
    local.set 0
    local.get 0
    global.set 0
    i32.const 4508649
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3428500
      call $j870 ;; 
      i32.const 3464612
      call $j870 ;; 
      i32.const 3464616
      call $j870 ;; 
      i32.const 3464608
      call $j870 ;; 
      i32.const 3423844
      call $j870 ;; 
      i32.const 3435240
      call $j870 ;; 
      i32.const 3435276
      call $j870 ;; 
      i32.const 3437220
      call $j870 ;; 
      i32.const 3539232
      call $j870 ;; 
      i32.const 3548512
      call $j870 ;; 
      i32.const 4508649
      i32.const 1
      i32.store8
    end
    local.get 0
    i32.const 0
    i32.store offset=12
    i32.const 3435240
    i32.load
    call $j871 ;; 
    block  ;; label = @1
      i32.const 3435240
      i32.load
      i32.load offset=92
      i32.load
      i32.load8_u offset=135
      i32.eqz
      if  ;; label = @2
        i32.const 3428500
        i32.load
        call $j871 ;; 
        i32.const 3428500
        i32.load
        i32.load offset=92
        i32.load offset=60
        local.set 1
        br 1 (;@1;)
      end
      i32.const 3437220
      i32.load
      call $j871 ;; 
      local.get 0
      i32.const 3548512
      i32.load
      i32.const 0
      i32.const 0
      call $j3942 ;; ServerFileCreate.GetServerFileIntVar
      block  ;; label = @2
        local.get 0
        i32.load
        i32.const 1
        i32.and
        local.set 1
        local.get 1
        if  ;; label = @3
          local.get 1
          local.get 0
          i32.load offset=4
          i32.eqz
          i32.and
          i32.eqz
          br_if 1 (;@2;)
        end
        i32.const 3428500
        i32.load
        call $j871 ;; 
        i32.const 3428500
        i32.load
        i32.load offset=92
        i32.load offset=60
        local.set 1
        br 1 (;@1;)
      end
      i32.const 3539232
      i32.load
      local.set 1
      local.get 1
      i32.const 0
      call $j1484 ;; 
      i32.eqz
      if  ;; label = @2
        i32.const 3423844
        i32.load
        call $j873 ;; 
        local.set 2
        local.get 2
        i32.const 3464608
        i32.load
        call $j1967 ;; 
        i32.const 3435240
        i32.load
        call $j871 ;; 
        local.get 0
        local.get 1
        f32.const 0x1.7cp+6 (;=95;)
        i32.const 0
        call $j19385 ;; NetInitManager.GetABWeight
        local.get 2
        local.get 0
        f32.load offset=4
        i32.const 0
        i32.const 3464612
        i32.load
        call $j1709 ;; 
        local.get 2
        f32.const 0x1.4p+2 (;=5;)
        i32.const 1
        i32.const 3464612
        i32.load
        call $j1709 ;; 
        local.get 2
        i32.const 3464616
        i32.load
        call $j1898 ;; 
        local.set 2
        local.get 0
        local.get 2
        i32.store offset=12
        local.get 1
        local.get 2
        i32.const 0
        call $j1296 ;; 
        i32.const 3435276
        i32.load
        local.set 2
        local.get 0
        i32.const -1
        i32.store offset=4
        local.get 0
        local.get 2
        i32.store
        local.get 0
        local.get 0
        i32.load offset=12
        i32.store offset=8
        local.get 0
        i32.const 0
        call $j956 ;; 
        i32.const 0
        call $j31862 ;; ThinkingManager.SetAnimSysABPack
      end
      block  ;; label = @2
        block  ;; label = @3
          local.get 1
          i32.const 0
          i32.const 0
          call $j1852 ;; 
          local.set 1
          local.get 1
          br_table 2 (;@1;) 1 (;@2;) 0 (;@3;)
        end
        i32.const 3428500
        i32.load
        call $j871 ;; 
        i32.const 3428500
        i32.load
        i32.load offset=92
        i32.load offset=60
        local.set 1
        br 1 (;@1;)
      end
      i32.const 1
      local.set 1
    end
    local.get 0
    i32.const 16
    i32.add
    global.set 0
    local.get 1
    return)
