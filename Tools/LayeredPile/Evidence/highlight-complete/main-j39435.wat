  (func $j39435 (type 5) (param i32 i32 i32) (result i32)
    (local i32 i32 i32 i32)
    global.get 0
    i32.const 16
    i32.sub
    local.set 4
    local.get 4
    global.set 0
    i32.const 4505062
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3429928
      call $j870 ;; 
      i32.const 3451196
      call $j870 ;; 
      i32.const 3451200
      call $j870 ;; 
      i32.const 3432324
      call $j870 ;; 
      i32.const 3432484
      call $j870 ;; 
      i32.const 3468552
      call $j870 ;; 
      i32.const 3468556
      call $j870 ;; 
      i32.const 3435416
      call $j870 ;; 
      i32.const 3540124
      call $j870 ;; 
      i32.const 3534828
      call $j870 ;; 
      i32.const 4505062
      i32.const 1
      i32.store8
    end
    local.get 4
    i32.const 0
    i32.store offset=12
    i32.const 3432484
    i32.load
    call $j871 ;; 
    i32.const 1
    local.set 2
    i32.const 3468552
    i32.load
    drop
    i32.const 3432484
    i32.load
    i32.load offset=92
    i32.load offset=132
    i32.load offset=12
    local.set 5
    block  ;; label = @1
      local.get 5
      i32.eqz
      br_if 0 (;@1;)
      local.get 0
      i32.load offset=420
      i32.eqz
      br_if 0 (;@1;)
      i32.const 0
      local.set 2
      block  ;; label = @2
        loop  ;; label = @3
          local.get 2
          local.get 5
          i32.ge_s
          br_if 1 (;@2;)
          i32.const 3432484
          i32.load
          call $j871 ;; 
          i32.const 3432484
          i32.load
          i32.load offset=92
          i32.load offset=132
          local.get 2
          i32.const 3468556
          i32.load
          call $j895 ;; 
          local.set 3
          i32.const 3435416
          i32.load
          call $j871 ;; 
          block  ;; label = @4
            block  ;; label = @5
              local.get 3
              i32.const 0
              i32.const 0
              call $j924 ;; 
              br_if 0 (;@5;)
              local.get 3
              i32.const 0
              call $j1834 ;; 
              i32.eqz
              br_if 0 (;@5;)
              local.get 3
              i32.load offset=12
              i32.const 1
              i32.eq
              br_if 1 (;@4;)
            end
            local.get 2
            i32.const 1
            i32.add
            local.set 2
            br 1 (;@3;)
          end
        end
        i32.const 0
        local.set 2
        i32.const 20511
        local.get 0
        local.get 1
        i32.const 0
        call $redir_iiii
        br_if 1 (;@1;)
      end
      i32.const 3432484
      i32.load
      call $j871 ;; 
      i32.const 3432484
      i32.load
      i32.load offset=92
      i32.load offset=136
      local.get 1
      local.get 4
      i32.const 12
      i32.add
      i32.const 3451196
      i32.load
      call $j1012 ;; 
      drop
      i32.const 1
      local.set 2
      i32.const 0
      call $j2109 ;; 
      local.set 3
      local.get 3
      local.get 4
      i32.load offset=12
      i32.eq
      br_if 0 (;@1;)
      i32.const 3432484
      i32.load
      call $j871 ;; 
      i32.const 3432484
      i32.load
      i32.load offset=92
      i32.load offset=136
      local.get 1
      local.get 3
      i32.const 3451200
      i32.load
      call $j1871 ;; 
      i32.const 3432484
      i32.load
      i32.load offset=92
      i32.load offset=140
      i32.eqz
      if  ;; label = @2
        i32.const 3429928
        i32.load
        call $j873 ;; 
        local.set 1
        local.get 1
        i32.const 0
        call $j5323 ;; 
        i32.const 3432484
        i32.load
        call $j871 ;; 
        i32.const 3432484
        i32.load
        i32.load offset=92
        local.get 1
        i32.store offset=140
        i32.const 3432484
        i32.load
        i32.load offset=92
        drop
        i32.const 3432484
        i32.load
        i32.load offset=92
        i32.load offset=140
        i32.const 3540124
        i32.load
        i32.const 0
        call $j5322 ;; 
      end
      i32.const 3432484
      i32.load
      call $j871 ;; 
      i32.const 3432484
      i32.load
      i32.load offset=92
      i32.load offset=144
      local.set 1
      i32.const 3435416
      i32.load
      call $j871 ;; 
      local.get 1
      i32.const 0
      i32.const 0
      call $j924 ;; 
      if  ;; label = @2
        i32.const 3432484
        i32.load
        call $j871 ;; 
        local.get 0
        i32.const 3432484
        i32.load
        i32.load offset=92
        i32.const 144
        i32.add
        i32.const 3534828
        i32.load
        i32.const 1
        call $j3068 ;; HighlightEffect.InitMaterial
        i32.const 3432484
        i32.load
        i32.load offset=92
        i32.load offset=144
        local.set 0
        i32.const 3435416
        i32.load
        call $j871 ;; 
        local.get 0
        i32.const 0
        i32.const 0
        call $j924 ;; 
        br_if 1 (;@1;)
      end
      i32.const 3432484
      i32.load
      call $j871 ;; 
      i32.const 3432484
      i32.load
      i32.load offset=92
      i32.load offset=140
      i32.const 0
      call $j3233 ;; 
      loop  ;; label = @2
        block  ;; label = @3
          i32.const 3432484
          i32.load
          call $j871 ;; 
          i32.const 3432484
          i32.load
          i32.load offset=92
          local.set 0
          local.get 6
          local.get 5
          i32.ge_s
          br_if 0 (;@3;)
          local.get 0
          i32.load offset=132
          local.get 6
          i32.const 3468556
          i32.load
          call $j895 ;; 
          local.set 0
          i32.const 3435416
          i32.load
          call $j871 ;; 
          block  ;; label = @4
            local.get 0
            i32.const 0
            i32.const 0
            call $j924 ;; 
            br_if 0 (;@4;)
            local.get 0
            i32.const 0
            call $j1834 ;; 
            i32.eqz
            br_if 0 (;@4;)
            local.get 0
            i32.load offset=12
            br_if 0 (;@4;)
            local.get 0
            i32.load offset=16
            local.set 1
            local.get 1
            i32.eqz
            br_if 0 (;@4;)
            i32.const 0
            local.set 3
            local.get 1
            i32.load offset=12
            local.set 2
            local.get 2
            i32.eqz
            br_if 0 (;@4;)
            loop  ;; label = @5
              local.get 2
              local.get 3
              i32.le_s
              br_if 1 (;@4;)
              i32.const 0
              local.set 2
              block  ;; label = @6
                local.get 1
                local.get 3
                i32.const 3
                i32.shl
                i32.add
                i32.load offset=16
                local.set 1
                local.get 1
                i32.const 0
                call $j7852 ;; 
                i32.eqz
                br_if 0 (;@6;)
                loop  ;; label = @7
                  local.get 2
                  local.get 0
                  i32.load offset=16
                  local.get 3
                  i32.const 3
                  i32.shl
                  i32.add
                  i32.load offset=20
                  i32.ge_s
                  br_if 1 (;@6;)
                  i32.const 3432484
                  i32.load
                  call $j871 ;; 
                  i32.const 3432484
                  i32.load
                  i32.load offset=92
                  i32.load offset=140
                  local.get 1
                  i32.const 3432484
                  i32.load
                  i32.load offset=92
                  i32.load offset=144
                  local.get 2
                  i32.const 0
                  call $j4805 ;; 
                  local.get 2
                  i32.const 1
                  i32.add
                  local.set 2
                  br 0 (;@7;)
                end
                unreachable
              end
              local.get 3
              i32.const 1
              i32.add
              local.set 3
              local.get 0
              i32.load offset=16
              local.set 1
              local.get 1
              i32.load offset=12
              local.set 2
              br 0 (;@5;)
            end
            unreachable
          end
          local.get 6
          i32.const 1
          i32.add
          local.set 6
          br 1 (;@2;)
        end
      end
      local.get 0
      i32.load offset=140
      local.set 0
      i32.const 3432324
      i32.load
      call $j871 ;; 
      local.get 0
      i32.const 0
      call $j3211 ;; 
      i32.const 1
      local.set 2
    end
    local.get 4
    i32.const 16
    i32.add
    global.set 0
    local.get 2
    return)
