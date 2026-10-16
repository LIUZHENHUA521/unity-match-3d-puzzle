  (func $j20236 (type 7) (param i32 i32 i32 i32)
    (local i32 i32 i32 i32 i32 i32 i32 f32 f32 f32 f32)
    global.get 0
    i32.const 160
    i32.sub
    local.set 3
    local.get 3
    global.set 0
    i32.const 4505051
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3495728
      call $j870 ;; 
      i32.const 3435416
      call $j870 ;; 
      i32.const 4505051
      i32.const 1
      i32.store8
    end
    local.get 0
    i32.load offset=416
    local.get 1
    i32.const 164
    i32.mul
    i32.add
    i32.const 16
    i32.add
    i32.load offset=52
    local.set 9
    i32.const 3435416
    i32.load
    call $j871 ;; 
    block  ;; label = @1
      local.get 9
      i32.const 0
      i32.const 0
      call $j924 ;; 
      br_if 0 (;@1;)
      local.get 0
      i32.load offset=416
      local.get 1
      i32.const 164
      i32.mul
      i32.add
      i32.const 16
      i32.add
      i32.load offset=4
      local.set 9
      local.get 3
      i32.const 144
      i32.add
      local.get 9
      i32.const 0
      call $j1387 ;; 
      local.get 3
      f32.load offset=152
      local.set 11
      local.get 3
      f32.load offset=148
      local.set 12
      local.get 0
      i32.load offset=416
      local.get 1
      i32.const 164
      i32.mul
      i32.add
      i32.const 16
      i32.add
      local.set 7
      block  ;; label = @2
        local.get 3
        f32.load offset=144
        f32.const 0x0p+0 (;=0;)
        f32.ge
        i32.eqz
        br_if 0 (;@2;)
        local.get 12
        f32.const 0x0p+0 (;=0;)
        f32.ge
        i32.eqz
        br_if 0 (;@2;)
        local.get 11
        f32.const 0x0p+0 (;=0;)
        f32.ge
        i32.eqz
        br_if 0 (;@2;)
        local.get 7
        i32.const 0
        i32.store8 offset=8
        br 1 (;@1;)
      end
      block  ;; label = @2
        local.get 2
        i32.eqz
        if  ;; label = @3
          local.get 7
          i32.load offset=52
          local.set 7
          br 1 (;@2;)
        end
        local.get 7
        i32.load offset=56
        local.set 2
        i32.const 3435416
        i32.load
        call $j871 ;; 
        local.get 2
        i32.const 3495728
        i32.load
        call $j1764 ;; 
        local.set 7
      end
      i32.const 39288
      local.get 7
      i32.const 0
      call $redir_iii
      local.set 8
      i32.const 0
      local.set 2
      loop  ;; label = @2
        local.get 8
        i32.load offset=12
        local.get 2
        i32.gt_s
        if  ;; label = @3
          local.get 8
          local.get 2
          i32.const 12
          i32.mul
          i32.add
          local.set 6
          local.get 3
          local.get 6
          i32.load offset=24
          i32.store offset=152
          local.get 6
          local.set 4
          local.get 4
          i32.load offset=16
          local.set 5
          local.get 4
          i32.load offset=20
          local.set 4
          local.get 3
          local.get 5
          i32.store offset=144
          local.get 3
          local.get 4
          i32.store offset=148
          local.get 3
          local.get 3
          i32.load offset=152
          i32.store offset=24
          local.get 3
          i32.load offset=144
          local.set 4
          local.get 3
          i32.load offset=148
          local.set 5
          local.get 3
          local.get 4
          i32.store offset=16
          local.get 3
          local.get 5
          i32.store offset=20
          local.get 3
          i32.const 128
          i32.add
          local.get 9
          local.get 3
          i32.const 16
          i32.add
          i32.const 0
          call $j1344 ;; 
          local.get 3
          i32.load offset=136
          local.set 6
          local.get 3
          local.get 6
          i32.store offset=120
          local.get 3
          local.get 6
          i32.store offset=8
          local.get 3
          i32.load offset=128
          local.set 5
          local.get 3
          i32.load offset=132
          local.set 4
          local.get 3
          local.get 5
          i32.store offset=112
          local.get 3
          local.get 4
          i32.store offset=116
          local.get 3
          local.get 5
          i32.store
          local.get 3
          local.get 4
          i32.store offset=4
          local.get 8
          local.get 2
          i32.const 12
          i32.mul
          i32.add
          local.set 6
          local.get 6
          local.get 3
          i32.load offset=8
          i32.store offset=24
          local.get 3
          i32.load
          local.set 4
          local.get 3
          i32.load offset=4
          local.set 5
          local.get 4
          local.set 10
          local.get 6
          local.set 4
          local.get 4
          local.get 10
          i32.store offset=16
          local.get 4
          local.get 5
          i32.store offset=20
          local.get 2
          i32.const 1
          i32.add
          local.set 2
          br 1 (;@2;)
        end
      end
      local.get 7
      local.get 8
      i32.const 0
      call $j2261 ;; 
      i32.const 39290
      local.get 7
      i32.const 0
      call $redir_iii
      local.set 8
      local.get 8
      if  ;; label = @2
        i32.const 0
        local.set 2
        loop  ;; label = @3
          local.get 8
          i32.load offset=12
          local.get 2
          i32.gt_s
          if  ;; label = @4
            local.get 8
            local.get 2
            i32.const 12
            i32.mul
            i32.add
            local.set 6
            local.get 3
            local.get 6
            i32.load offset=24
            i32.store offset=152
            local.get 6
            local.set 4
            local.get 4
            i32.load offset=16
            local.set 5
            local.get 4
            i32.load offset=20
            local.set 4
            local.get 3
            local.get 5
            i32.store offset=144
            local.get 3
            local.get 4
            i32.store offset=148
            local.get 3
            local.get 3
            i32.load offset=152
            i32.store offset=56
            local.get 3
            i32.load offset=144
            local.set 4
            local.get 3
            i32.load offset=148
            local.set 5
            local.get 3
            local.get 4
            i32.store offset=48
            local.get 3
            local.get 5
            i32.store offset=52
            local.get 3
            i32.const 128
            i32.add
            local.get 9
            local.get 3
            i32.const 48
            i32.add
            i32.const 0
            call $j9136 ;; 
            local.get 3
            f32.load offset=136
            local.set 11
            local.get 3
            f32.load offset=132
            local.set 12
            local.get 3
            f32.load offset=128
            local.set 13
            i32.const 4523251
            i32.load8_u
            i32.eqz
            if  ;; label = @5
              i32.const 3434688
              call $j870 ;; 
              i32.const 4523251
              i32.const 1
              i32.store8
            end
            i32.const 3434688
            i32.load
            call $j871 ;; 
            local.get 13
            local.get 13
            f32.mul
            local.get 12
            local.get 12
            f32.mul
            f32.add
            local.get 11
            local.get 11
            f32.mul
            f32.add
            f32.sqrt
            local.set 14
            block  ;; label = @5
              local.get 14
              f32.const 0x1.4f8b58p-17 (;=1e-05;)
              f32.gt
              if  ;; label = @6
                local.get 11
                local.get 14
                f32.div
                local.set 11
                local.get 13
                local.get 14
                f32.div
                local.set 13
                local.get 12
                local.get 14
                f32.div
                local.set 12
                br 1 (;@5;)
              end
              i32.const 4525920
              i32.load8_u
              i32.eqz
              if  ;; label = @6
                i32.const 3439908
                call $j870 ;; 
                i32.const 4525920
                i32.const 1
                i32.store8
              end
              i32.const 3439908
              i32.load
              i32.load offset=92
              local.set 6
              local.get 6
              f32.load offset=8
              local.set 11
              local.get 6
              f32.load
              local.set 13
              local.get 6
              f32.load offset=4
              local.set 12
            end
            local.get 3
            local.get 11
            f32.store offset=104
            local.get 3
            local.get 3
            i32.load offset=104
            i32.store offset=40
            local.get 3
            local.get 12
            f32.store offset=100
            local.get 3
            local.get 13
            f32.store offset=96
            local.get 3
            i32.load offset=96
            local.set 5
            local.get 3
            i32.load offset=100
            local.set 4
            local.get 3
            local.get 5
            i32.store offset=32
            local.get 3
            local.get 4
            i32.store offset=36
            local.get 8
            local.get 2
            i32.const 12
            i32.mul
            i32.add
            local.set 6
            local.get 6
            local.get 3
            i32.load offset=40
            i32.store offset=24
            local.get 3
            i32.load offset=32
            local.set 4
            local.get 3
            i32.load offset=36
            local.set 5
            local.get 4
            local.set 10
            local.get 6
            local.set 4
            local.get 4
            local.get 10
            i32.store offset=16
            local.get 4
            local.get 5
            i32.store offset=20
            local.get 2
            i32.const 1
            i32.add
            local.set 2
            br 1 (;@3;)
          end
        end
        local.get 7
        local.get 8
        i32.const 0
        call $j4694 ;; 
      end
      local.get 7
      i32.const 0
      call $j2690 ;; 
      local.get 0
      i32.load offset=416
      local.get 1
      i32.const 164
      i32.mul
      i32.add
      i32.const 16
      i32.add
      local.get 7
      i32.store offset=52
      local.get 0
      i32.load offset=416
      local.get 1
      i32.const 164
      i32.mul
      i32.add
      i32.const 16
      i32.add
      i32.const 1
      i32.store8 offset=8
      local.get 0
      i32.load offset=416
      local.set 2
      local.get 3
      i32.const 128
      i32.add
      local.get 9
      i32.const 0
      call $j1028 ;; 
      local.get 3
      local.get 3
      i32.load offset=136
      i32.store offset=152
      local.get 3
      i32.load offset=128
      local.set 5
      local.get 3
      i32.load offset=132
      local.set 4
      local.get 3
      local.get 5
      i32.store offset=144
      local.get 3
      local.get 4
      i32.store offset=148
      local.get 2
      local.get 1
      i32.const 164
      i32.mul
      i32.add
      i32.const 16
      i32.add
      local.set 2
      local.get 2
      local.get 3
      i32.load offset=152
      i32.store offset=20
      local.get 3
      i32.load offset=144
      local.set 4
      local.get 3
      i32.load offset=148
      local.set 5
      local.get 4
      local.set 6
      local.get 2
      local.set 4
      local.get 4
      local.get 6
      i32.store offset=12
      local.get 4
      local.get 5
      i32.store offset=16
      local.get 0
      i32.load offset=416
      local.set 2
      local.get 3
      i32.const 80
      i32.add
      local.get 9
      i32.const 0
      call $j5017 ;; 
      local.get 3
      local.get 3
      i32.load offset=88
      i32.store offset=136
      local.get 3
      i32.load offset=80
      local.set 5
      local.get 3
      i32.load offset=84
      local.set 4
      local.get 3
      local.get 5
      i32.store offset=128
      local.get 3
      local.get 4
      i32.store offset=132
      local.get 2
      local.get 1
      i32.const 164
      i32.mul
      i32.add
      i32.const 16
      i32.add
      local.set 2
      local.get 2
      local.get 3
      i32.load offset=136
      i32.store offset=32
      local.get 3
      i32.load offset=128
      local.set 4
      local.get 3
      i32.load offset=132
      local.set 5
      local.get 4
      local.set 6
      local.get 2
      local.set 4
      local.get 4
      local.get 6
      i32.store offset=24
      local.get 4
      local.get 5
      i32.store offset=28
      local.get 0
      i32.load offset=416
      local.set 0
      local.get 3
      i32.const -64
      i32.sub
      local.get 9
      i32.const 0
      call $j3154 ;; 
      local.get 3
      local.get 3
      i32.load offset=72
      i32.store offset=88
      local.get 3
      i32.load offset=64
      local.set 5
      local.get 3
      i32.load offset=68
      local.set 4
      local.get 3
      local.get 5
      i32.store offset=80
      local.get 3
      local.get 4
      i32.store offset=84
      local.get 0
      local.get 1
      i32.const 164
      i32.mul
      i32.add
      i32.const 16
      i32.add
      local.set 0
      local.get 0
      local.get 3
      i32.load offset=88
      i32.store offset=44
      local.get 3
      i32.load offset=80
      local.set 4
      local.get 3
      i32.load offset=84
      local.set 5
      local.get 4
      local.set 1
      local.get 0
      local.set 4
      local.get 4
      local.get 1
      i32.store offset=36
      local.get 4
      local.get 5
      i32.store offset=40
    end
    local.get 3
    i32.const 160
    i32.add
    global.set 0)
