  (func $j39438 (type 8) (param i32 i32 i32 i32 i32)
    (local i32 i32)
    global.get 0
    i32.const 128
    i32.sub
    local.set 4
    local.get 4
    global.set 0
    i32.const 4505033
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3432324
      call $j870 ;; 
      i32.const 3437276
      call $j870 ;; 
      i32.const 4505033
      i32.const 1
      i32.store8
    end
    block  ;; label = @1
      block  ;; label = @2
        local.get 0
        i32.load offset=416
        local.get 1
        i32.const 164
        i32.mul
        i32.add
        i32.const 16
        i32.add
        local.set 5
        local.get 5
        i32.load8_u offset=160
        if  ;; label = @3
          i32.const 0
          local.set 5
        else
          local.get 5
          i32.load offset=68
          i32.const 1
          i32.eq
          local.set 5
        end
        local.get 5
        i32.eqz
        if  ;; label = @3
          i32.const 8
          i32.const 4
          local.get 3
          select
          local.set 5
          i32.const 0
          local.set 3
          br 1 (;@2;)
        end
        local.get 0
        i32.load offset=480
        i32.const 0
        call $j3233 ;; 
        i32.const 8
        i32.const 4
        local.get 3
        select
        local.set 5
        i32.const 0
        local.set 3
        loop  ;; label = @3
          local.get 2
          i32.const 0
          call $j1782 ;; 
          local.get 3
          i32.gt_s
          if  ;; label = @4
            local.get 0
            i32.load offset=72
            local.get 3
            i32.shr_u
            i32.const 1
            i32.and
            if  ;; label = @5
              local.get 0
              i32.load offset=416
              local.get 1
              i32.const 164
              i32.mul
              i32.add
              i32.const 16
              i32.add
              i32.load offset=72
              local.get 3
              i32.const 2
              i32.shl
              i32.add
              i32.load offset=16
              local.set 6
              i32.const 3437276
              i32.load
              call $j871 ;; 
              local.get 6
              i32.const 3437276
              i32.load
              i32.load offset=92
              i32.load offset=20
              local.get 5
              i32.const 0
              call $j1524 ;; 
              local.get 0
              i32.load offset=480
              local.get 0
              i32.load offset=416
              local.get 1
              i32.const 164
              i32.mul
              i32.add
              i32.const 16
              i32.add
              i32.load offset=60
              local.get 0
              i32.load offset=416
              local.get 1
              i32.const 164
              i32.mul
              i32.add
              i32.const 16
              i32.add
              i32.load offset=72
              local.get 3
              i32.const 2
              i32.shl
              i32.add
              i32.load offset=16
              local.get 3
              i32.const 0
              i32.const 0
              call $j4806 ;; 
            end
            local.get 3
            i32.const 1
            i32.add
            local.set 3
            br 1 (;@3;)
          end
        end
        local.get 0
        i32.load offset=480
        local.set 0
        i32.const 3432324
        i32.load
        call $j871 ;; 
        local.get 0
        i32.const 0
        call $j3211 ;; 
        br 1 (;@1;)
      end
      loop  ;; label = @2
        local.get 2
        i32.const 0
        call $j1782 ;; 
        local.get 3
        i32.le_s
        br_if 1 (;@1;)
        local.get 0
        i32.load offset=72
        local.get 3
        i32.shr_u
        i32.const 1
        i32.and
        if  ;; label = @3
          local.get 0
          i32.load offset=416
          local.get 1
          i32.const 164
          i32.mul
          i32.add
          i32.const 16
          i32.add
          i32.load offset=72
          local.get 3
          i32.const 2
          i32.shl
          i32.add
          i32.load offset=16
          local.set 6
          i32.const 3437276
          i32.load
          call $j871 ;; 
          local.get 6
          i32.const 3437276
          i32.load
          i32.load offset=92
          i32.load offset=20
          local.get 5
          i32.const 0
          call $j1524 ;; 
          local.get 0
          i32.load offset=416
          local.get 1
          i32.const 164
          i32.mul
          i32.add
          i32.const 16
          i32.add
          i32.load offset=72
          local.get 3
          i32.const 2
          i32.shl
          i32.add
          i32.load offset=16
          i32.const 0
          i32.const 0
          call $j3054 ;; 
          drop
          local.get 4
          i32.const -64
          i32.sub
          local.get 0
          i32.load offset=416
          local.get 1
          i32.const 164
          i32.mul
          i32.add
          i32.const 112
          i32.add
          i32.const 64
          call $j883 ;; 
          drop
          i32.const 3432324
          i32.load
          call $j871 ;; 
          local.get 2
          local.get 4
          local.get 4
          i32.const -64
          i32.sub
          i32.const 64
          call $j883 ;; 
          local.get 3
          i32.const 0
          call $j4006 ;; 
        end
        local.get 3
        i32.const 1
        i32.add
        local.set 3
        br 0 (;@2;)
      end
      unreachable
    end
    local.get 4
    i32.const 128
    i32.add
    global.set 0)
