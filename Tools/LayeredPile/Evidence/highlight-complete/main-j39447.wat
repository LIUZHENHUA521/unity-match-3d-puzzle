  (func $j39447 (type 0) (param i32 i32)
    (local i32 i32 i32 i32)
    global.get 0
    i32.const 464
    i32.sub
    local.set 2
    local.get 2
    global.set 0
    i32.const 4505058
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3427308
      call $j870 ;; 
      i32.const 3452652
      call $j870 ;; 
      i32.const 3452656
      call $j870 ;; 
      i32.const 3432484
      call $j870 ;; 
      i32.const 3434776
      call $j870 ;; 
      i32.const 3435416
      call $j870 ;; 
      i32.const 4505058
      i32.const 1
      i32.store8
    end
    local.get 2
    i32.const 400
    i32.add
    i32.const 0
    i32.const 64
    call $j890 ;; 
    drop
    local.get 2
    i32.const 0
    i32.store offset=396
    i32.const 3432484
    i32.load
    call $j871 ;; 
    block  ;; label = @1
      block  ;; label = @2
        i32.const 3432484
        i32.load
        i32.load offset=92
        i32.load offset=92
        i32.eqz
        if  ;; label = @3
          local.get 0
          i32.load offset=420
          local.set 1
          br 1 (;@2;)
        end
        i32.const 3432484
        i32.load
        call $j871 ;; 
        local.get 0
        i32.load offset=420
        local.set 1
        local.get 1
        i32.const 3432484
        i32.load
        i32.load offset=92
        i32.load offset=92
        i32.load offset=12
        i32.eq
        br_if 1 (;@1;)
      end
      i32.const 3427308
      i32.load
      local.get 1
      call $j888 ;; 
      local.set 1
      i32.const 3432484
      i32.load
      call $j871 ;; 
      i32.const 3432484
      i32.load
      i32.load offset=92
      local.get 1
      i32.store offset=92
      i32.const 3432484
      i32.load
      i32.load offset=92
      drop
    end
    local.get 0
    i32.const 0
    i32.store offset=864
    local.get 2
    i32.const 400
    i32.add
    local.get 0
    i32.const 792
    i32.add
    i32.const 64
    call $j883 ;; 
    drop
    i32.const 0
    local.set 1
    i32.const -1
    local.set 4
    loop  ;; label = @1
      local.get 0
      i32.load offset=420
      local.get 1
      i32.gt_s
      if  ;; label = @2
        i32.const 3432484
        i32.load
        call $j871 ;; 
        i32.const 3432484
        i32.load
        i32.load offset=92
        i32.load offset=92
        local.get 1
        i32.const 104
        i32.mul
        i32.add
        i32.const 16
        i32.add
        i32.const 0
        i32.const 0
        call $j10336 ;; 
        block  ;; label = @3
          local.get 0
          i32.load offset=416
          local.get 1
          i32.const 164
          i32.mul
          i32.add
          i32.load8_u offset=80
          br_if 0 (;@3;)
          local.get 0
          i32.load offset=416
          local.get 1
          i32.const 164
          i32.mul
          i32.add
          i32.load offset=68
          local.set 3
          i32.const 3435416
          i32.load
          call $j871 ;; 
          local.get 3
          i32.const 0
          i32.const 0
          call $j915 ;; 
          i32.eqz
          br_if 0 (;@3;)
          local.get 3
          i32.const 0
          call $j13101 ;; 
          i32.eqz
          br_if 0 (;@3;)
          local.get 3
          i32.const 0
          call $j2542 ;; 
          i32.const 65535
          i32.gt_s
          br_if 0 (;@3;)
          i32.const 3432484
          i32.load
          call $j871 ;; 
          i32.const 3432484
          i32.load
          i32.load offset=92
          i32.load offset=92
          local.get 1
          i32.const 104
          i32.mul
          i32.add
          i32.const 16
          i32.add
          local.get 3
          i32.const 0
          call $j10336 ;; 
          local.get 0
          i32.load offset=416
          local.get 1
          i32.const 164
          i32.mul
          i32.add
          i32.load offset=76
          i32.const 0
          call $j925 ;; 
          i32.const 0
          call $j1145 ;; 
          local.set 3
          block  ;; label = @4
            local.get 4
            i32.const -1
            i32.le_s
            if  ;; label = @5
              local.get 0
              local.get 3
              i32.store offset=864
              local.get 2
              i32.const 328
              i32.add
              local.get 0
              i32.load offset=416
              local.get 1
              i32.const 164
              i32.mul
              i32.add
              i32.load offset=20
              i32.const 0
              call $j6368 ;; 
              local.get 2
              i32.const 400
              i32.add
              local.get 2
              i32.const 328
              i32.add
              i32.const 64
              call $j883 ;; 
              drop
              local.get 1
              local.set 4
              br 1 (;@4;)
            end
            local.get 0
            local.get 0
            i32.load offset=864
            local.get 3
            i32.xor
            i32.store offset=864
            local.get 0
            i32.load offset=416
            local.get 1
            i32.const 164
            i32.mul
            i32.add
            i32.const 0
            i32.store offset=68
          end
          i32.const 3432484
          i32.load
          call $j871 ;; 
          i32.const 3432484
          i32.load
          i32.load offset=92
          i32.load offset=92
          local.set 3
          local.get 2
          i32.const 328
          i32.add
          local.get 0
          i32.load offset=416
          local.get 1
          i32.const 164
          i32.mul
          i32.add
          i32.load offset=20
          i32.const 0
          call $j3155 ;; 
          local.get 2
          i32.const 200
          i32.add
          local.get 2
          i32.const 328
          i32.add
          i32.const 64
          call $j883 ;; 
          drop
          local.get 2
          i32.const 136
          i32.add
          local.get 2
          i32.const 400
          i32.add
          i32.const 64
          call $j883 ;; 
          drop
          local.get 2
          i32.const 72
          i32.add
          local.get 2
          i32.const 200
          i32.add
          i32.const 64
          call $j883 ;; 
          drop
          local.get 2
          i32.const 264
          i32.add
          local.get 2
          i32.const 136
          i32.add
          local.get 2
          i32.const 72
          i32.add
          i32.const 0
          call $j3520 ;; 
          local.get 2
          i32.const 328
          i32.add
          local.get 2
          i32.const 264
          i32.add
          i32.const 64
          call $j883 ;; 
          drop
          local.get 3
          local.get 1
          i32.const 104
          i32.mul
          i32.add
          i32.const 16
          i32.add
          local.set 3
          local.get 2
          i32.const 8
          i32.add
          local.get 2
          i32.const 328
          i32.add
          i32.const 64
          call $j883 ;; 
          drop
          local.get 3
          local.get 2
          i32.const 8
          i32.add
          i32.const 0
          call $j10335 ;; 
          local.get 5
          i32.const 1
          i32.add
          local.set 5
        end
        local.get 1
        i32.const 1
        i32.add
        local.set 1
        br 1 (;@1;)
      end
    end
    local.get 5
    i32.const 2
    i32.ge_s
    if  ;; label = @1
      i32.const 3432484
      i32.load
      call $j871 ;; 
      block  ;; label = @2
        i32.const 3432484
        i32.load
        i32.load offset=92
        i32.load offset=124
        local.get 0
        i32.load offset=864
        local.get 2
        i32.const 396
        i32.add
        i32.const 3452652
        i32.load
        call $j1125 ;; 
        if  ;; label = @3
          local.get 2
          i32.load offset=396
          local.set 1
          i32.const 3435416
          i32.load
          call $j871 ;; 
          local.get 1
          i32.const 0
          i32.const 0
          call $j924 ;; 
          i32.eqz
          br_if 1 (;@2;)
        end
        i32.const 3434776
        i32.load
        call $j873 ;; 
        local.set 1
        local.get 1
        i32.const 0
        call $j2794 ;; 
        local.get 2
        local.get 1
        i32.store offset=396
        i32.const 3432484
        i32.load
        call $j871 ;; 
        local.get 1
        i32.const 3432484
        i32.load
        i32.load offset=92
        i32.load offset=92
        i32.const 1
        i32.const 1
        i32.const 0
        call $j19580 ;; 
        i32.const 3432484
        i32.load
        i32.load offset=92
        i32.load offset=124
        local.get 0
        i32.load offset=864
        local.get 2
        i32.load offset=396
        i32.const 3452656
        i32.load
        call $j1769 ;; 
      end
      local.get 0
      i32.load offset=416
      local.get 4
      i32.const 164
      i32.mul
      i32.add
      local.get 2
      i32.load offset=396
      i32.store offset=68
      local.get 0
      i32.load offset=416
      local.get 4
      i32.const 164
      i32.mul
      i32.add
      i32.const 1
      i32.store8 offset=176
    end
    local.get 2
    i32.const 464
    i32.add
    global.set 0)
