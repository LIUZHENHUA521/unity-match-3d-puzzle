  (func $j39455 (type 1) (param i32 i32 i32)
    (local i32 i32 i32 i32 i32 i32 i32 i32 f32 f32 f32 f32)
    global.get 0
    i32.const 128
    i32.sub
    local.set 2
    local.get 2
    global.set 0
    i32.const 4505056
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3458416
      call $j870 ;; 
      i32.const 3452652
      call $j870 ;; 
      i32.const 3458424
      call $j870 ;; 
      i32.const 3452656
      call $j870 ;; 
      i32.const 3458432
      call $j870 ;; 
      i32.const 3432484
      call $j870 ;; 
      i32.const 3427640
      call $j870 ;; 
      i32.const 3473656
      call $j870 ;; 
      i32.const 3473640
      call $j870 ;; 
      i32.const 3473684
      call $j870 ;; 
      i32.const 3473688
      call $j870 ;; 
      i32.const 3473692
      call $j870 ;; 
      i32.const 3425496
      call $j870 ;; 
      i32.const 3495728
      call $j870 ;; 
      i32.const 3435416
      call $j870 ;; 
      i32.const 3428200
      call $j870 ;; 
      i32.const 4505056
      i32.const 1
      i32.store8
    end
    local.get 2
    i32.const 0
    i32.store offset=124
    local.get 2
    i32.const 0
    i32.store offset=120
    i32.const 0
    local.set 5
    local.get 2
    i32.const 0
    i32.store offset=112
    local.get 2
    local.get 5
    i32.store offset=116
    local.get 2
    i32.const 0
    i32.store offset=108
    local.get 2
    i32.const 0
    i32.store offset=104
    i32.const 0
    local.set 5
    local.get 2
    i32.const 0
    i32.store offset=96
    local.get 2
    local.get 5
    i32.store offset=100
    local.get 0
    i32.load offset=416
    local.set 3
    block  ;; label = @1
      local.get 3
      i32.eqz
      br_if 0 (;@1;)
      local.get 3
      i32.load offset=12
      local.get 1
      i32.le_s
      br_if 0 (;@1;)
      local.get 3
      local.get 1
      i32.const 164
      i32.mul
      i32.add
      i32.load offset=68
      local.set 8
      local.get 8
      local.set 3
      local.get 3
      i32.load
      local.set 3
      local.get 3
      i32.load offset=204
      local.get 8
      local.get 3
      i32.load offset=208
      call $redir_iii
      local.set 10
      i32.const 3432484
      i32.load
      call $j871 ;; 
      block  ;; label = @2
        i32.const 3432484
        i32.load
        i32.load offset=92
        i32.load offset=116
        local.get 10
        local.get 2
        i32.const 124
        i32.add
        i32.const 3452652
        i32.load
        call $j1125 ;; 
        if  ;; label = @3
          local.get 2
          i32.load offset=124
          local.set 3
          i32.const 3435416
          i32.load
          call $j871 ;; 
          local.get 3
          i32.const 0
          i32.const 0
          call $j924 ;; 
          i32.eqz
          br_if 1 (;@2;)
        end
        local.get 8
        i32.const 0
        call $j13101 ;; 
        i32.eqz
        br_if 1 (;@1;)
        i32.const 3432484
        i32.load
        call $j871 ;; 
        block  ;; label = @3
          i32.const 3432484
          i32.load
          i32.load offset=92
          i32.load offset=100
          i32.eqz
          if  ;; label = @4
            i32.const 3425496
            i32.load
            call $j873 ;; 
            local.set 3
            local.get 3
            i32.const 3473640
            i32.load
            call $j918 ;; 
            i32.const 3432484
            i32.load
            call $j871 ;; 
            i32.const 3432484
            i32.load
            i32.load offset=92
            local.get 3
            i32.store offset=100
            i32.const 3432484
            i32.load
            i32.load offset=92
            drop
            br 1 (;@3;)
          end
          i32.const 3432484
          i32.load
          call $j871 ;; 
          i32.const 3432484
          i32.load
          i32.load offset=92
          i32.load offset=100
          i32.const 3473656
          i32.load
          call $j974 ;; 
        end
        i32.const 3432484
        i32.load
        call $j871 ;; 
        local.get 8
        i32.const 3432484
        i32.load
        i32.load offset=92
        i32.load offset=100
        i32.const 0
        call $j37651 ;; 
        i32.const 3473684
        i32.load
        drop
        i32.const 3432484
        i32.load
        i32.load offset=92
        i32.load offset=100
        i32.load offset=12
        local.set 3
        local.get 3
        i32.eqz
        br_if 1 (;@1;)
        i32.const 3432484
        i32.load
        call $j871 ;; 
        block  ;; label = @3
          i32.const 3432484
          i32.load
          i32.load offset=92
          i32.load offset=96
          i32.eqz
          if  ;; label = @4
            i32.const 3425496
            i32.load
            call $j873 ;; 
            local.set 6
            local.get 6
            i32.const 3473640
            i32.load
            call $j918 ;; 
            i32.const 3432484
            i32.load
            call $j871 ;; 
            i32.const 3432484
            i32.load
            i32.load offset=92
            local.get 6
            i32.store offset=96
            i32.const 3432484
            i32.load
            i32.load offset=92
            drop
            br 1 (;@3;)
          end
          i32.const 3432484
          i32.load
          call $j871 ;; 
          i32.const 3432484
          i32.load
          i32.load offset=92
          i32.load offset=96
          i32.const 3473656
          i32.load
          call $j974 ;; 
        end
        i32.const 3432484
        i32.load
        call $j871 ;; 
        local.get 8
        i32.const 3432484
        i32.load
        i32.load offset=92
        i32.load offset=96
        i32.const 0
        call $j19575 ;; 
        i32.const 3473684
        i32.load
        drop
        i32.const 3432484
        i32.load
        i32.load offset=92
        i32.load offset=96
        i32.load offset=12
        local.set 6
        i32.const 3432484
        i32.load
        call $j871 ;; 
        local.get 3
        local.get 6
        local.get 3
        local.get 6
        i32.lt_s
        select
        local.set 6
        block  ;; label = @3
          block  ;; label = @4
            i32.const 3432484
            i32.load
            i32.load offset=92
            i32.load offset=104
            if  ;; label = @5
              i32.const 3432484
              i32.load
              call $j871 ;; 
              i32.const 3432484
              i32.load
              i32.load offset=92
              i32.load offset=104
              i32.load offset=12
              local.get 6
              i32.ge_s
              br_if 1 (;@4;)
            end
            i32.const 3428200
            i32.load
            local.get 6
            call $j888 ;; 
            local.set 3
            i32.const 3432484
            i32.load
            call $j871 ;; 
            i32.const 3432484
            i32.load
            i32.load offset=92
            local.get 3
            i32.store offset=104
            i32.const 3432484
            i32.load
            i32.load offset=92
            drop
            br 1 (;@3;)
          end
          i32.const 4525920
          i32.load8_u
          i32.eqz
          if  ;; label = @4
            i32.const 3439908
            call $j870 ;; 
            i32.const 4525920
            i32.const 1
            i32.store8
          end
          i32.const 3439908
          i32.load
          i32.load offset=92
          local.set 3
          local.get 2
          local.get 3
          i32.load offset=8
          i32.store offset=120
          local.get 3
          i32.load
          local.set 5
          local.get 3
          i32.load offset=4
          local.set 7
          local.get 2
          local.get 5
          i32.store offset=112
          local.get 2
          local.get 7
          i32.store offset=116
          i32.const 0
          local.set 3
          loop  ;; label = @4
            local.get 3
            local.get 6
            i32.ge_s
            br_if 1 (;@3;)
            i32.const 3432484
            i32.load
            call $j871 ;; 
            i32.const 3432484
            i32.load
            i32.load offset=92
            i32.load offset=104
            local.set 4
            local.get 2
            local.get 2
            i32.load offset=120
            i32.store offset=56
            local.get 2
            i32.load offset=112
            local.set 7
            local.get 2
            i32.load offset=116
            local.set 5
            local.get 2
            local.get 7
            i32.store offset=48
            local.get 2
            local.get 5
            i32.store offset=52
            local.get 4
            local.get 3
            i32.const 12
            i32.mul
            i32.add
            local.set 4
            local.get 4
            local.get 2
            i32.load offset=56
            i32.store offset=24
            local.get 2
            i32.load offset=48
            local.set 5
            local.get 2
            i32.load offset=52
            local.set 7
            local.get 5
            local.set 9
            local.get 4
            local.set 5
            local.get 5
            local.get 9
            i32.store offset=16
            local.get 5
            local.get 7
            i32.store offset=20
            local.get 3
            i32.const 1
            i32.add
            local.set 3
            br 0 (;@4;)
          end
          unreachable
        end
        i32.const 3432484
        i32.load
        call $j871 ;; 
        block  ;; label = @3
          i32.const 3432484
          i32.load
          i32.load offset=92
          i32.load offset=108
          if  ;; label = @4
            i32.const 3432484
            i32.load
            call $j871 ;; 
            i32.const 3432484
            i32.load
            i32.load offset=92
            i32.load offset=108
            i32.load offset=12
            local.get 6
            i32.ge_s
            br_if 1 (;@3;)
          end
          i32.const 3427640
          i32.load
          local.get 6
          call $j888 ;; 
          local.set 3
          i32.const 3432484
          i32.load
          call $j871 ;; 
          i32.const 3432484
          i32.load
          i32.load offset=92
          local.get 3
          i32.store offset=108
          i32.const 3432484
          i32.load
          i32.load offset=92
          drop
        end
        i32.const 3432484
        i32.load
        call $j871 ;; 
        i32.const 3432484
        i32.load
        i32.load offset=92
        i32.load offset=112
        i32.const 3458416
        i32.load
        call $j1167 ;; 
        i32.const 0
        local.set 3
        loop  ;; label = @3
          local.get 3
          local.get 6
          i32.lt_s
          if  ;; label = @4
            i32.const 3432484
            i32.load
            call $j871 ;; 
            local.get 2
            i32.const 80
            i32.add
            i32.const 3432484
            i32.load
            i32.load offset=92
            i32.load offset=96
            local.get 3
            i32.const 3473688
            i32.load
            call $j986 ;; 
            local.get 2
            local.get 2
            i32.load offset=88
            i32.store offset=104
            local.get 2
            i32.load offset=80
            local.set 7
            local.get 2
            i32.load offset=84
            local.set 5
            local.get 2
            local.get 7
            i32.store offset=96
            local.get 2
            local.get 5
            i32.store offset=100
            i32.const 3432484
            i32.load
            i32.load offset=92
            i32.load offset=112
            local.set 4
            local.get 2
            local.get 2
            i32.load offset=104
            i32.store offset=24
            local.get 2
            i32.load offset=96
            local.set 5
            local.get 2
            i32.load offset=100
            local.set 7
            local.get 2
            local.get 5
            i32.store offset=16
            local.get 2
            local.get 7
            i32.store offset=20
            local.get 4
            local.get 2
            i32.const 16
            i32.add
            local.get 2
            i32.const 108
            i32.add
            i32.const 3458424
            i32.load
            call $j1012 ;; 
            i32.eqz
            if  ;; label = @5
              i32.const 3432484
              i32.load
              call $j871 ;; 
              i32.const 3432484
              i32.load
              i32.load offset=92
              i32.load offset=112
              local.set 4
              local.get 2
              local.get 2
              i32.load offset=104
              i32.store offset=8
              local.get 2
              local.get 3
              i32.store offset=108
              local.get 2
              i32.load offset=96
              local.set 7
              local.get 2
              i32.load offset=100
              local.set 5
              local.get 2
              local.get 7
              i32.store
              local.get 2
              local.get 5
              i32.store offset=4
              local.get 4
              local.get 2
              local.get 3
              i32.const 3458432
              i32.load
              call $j1871 ;; 
            end
            i32.const 3432484
            i32.load
            call $j871 ;; 
            i32.const 3432484
            i32.load
            i32.load offset=92
            i32.load offset=108
            local.get 3
            i32.const 2
            i32.shl
            i32.add
            local.get 2
            i32.load offset=108
            i32.store offset=16
            local.get 3
            i32.const 1
            i32.add
            local.set 3
            br 1 (;@3;)
          end
        end
        i32.const 0
        local.set 3
        loop  ;; label = @3
          local.get 3
          local.get 6
          i32.lt_s
          if  ;; label = @4
            i32.const 3432484
            i32.load
            call $j871 ;; 
            i32.const 3432484
            i32.load
            i32.load offset=92
            i32.load offset=104
            i32.const 3432484
            i32.load
            i32.load offset=92
            i32.load offset=108
            local.get 3
            i32.const 2
            i32.shl
            i32.add
            i32.load offset=16
            i32.const 12
            i32.mul
            i32.add
            local.set 4
            local.get 4
            i32.const 16
            i32.add
            f32.load offset=8
            local.set 13
            local.get 4
            f32.load offset=20
            local.set 11
            local.get 4
            f32.load offset=16
            local.set 12
            local.get 2
            i32.const 80
            i32.add
            i32.const 3432484
            i32.load
            i32.load offset=92
            i32.load offset=100
            local.get 3
            i32.const 3473688
            i32.load
            call $j986 ;; 
            local.get 12
            local.get 2
            f32.load offset=80
            f32.add
            local.set 12
            local.get 11
            local.get 2
            f32.load offset=84
            f32.add
            local.set 11
            local.get 4
            local.get 13
            local.get 2
            f32.load offset=88
            f32.add
            f32.store offset=24
            local.get 4
            local.get 11
            f32.store offset=20
            local.get 4
            local.get 12
            f32.store offset=16
            local.get 3
            i32.const 1
            i32.add
            local.set 3
            br 1 (;@3;)
          end
        end
        i32.const 0
        local.set 3
        loop  ;; label = @3
          local.get 3
          local.get 6
          i32.lt_s
          if  ;; label = @4
            i32.const 3432484
            i32.load
            call $j871 ;; 
            i32.const 3432484
            i32.load
            i32.load offset=92
            i32.load offset=100
            local.set 9
            i32.const 3432484
            i32.load
            i32.load offset=92
            i32.load offset=104
            i32.const 3432484
            i32.load
            i32.load offset=92
            i32.load offset=108
            local.get 3
            i32.const 2
            i32.shl
            i32.add
            i32.load offset=16
            i32.const 12
            i32.mul
            i32.add
            local.set 4
            local.get 4
            i32.const 16
            i32.add
            f32.load offset=8
            local.set 13
            local.get 4
            f32.load offset=20
            local.set 11
            local.get 4
            f32.load offset=16
            local.set 12
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
            local.get 12
            local.get 12
            f32.mul
            local.get 11
            local.get 11
            f32.mul
            f32.add
            local.get 13
            local.get 13
            f32.mul
            f32.add
            f32.sqrt
            local.set 14
            block  ;; label = @5
              local.get 14
              f32.const 0x1.4f8b58p-17 (;=1e-05;)
              f32.gt
              if  ;; label = @6
                local.get 13
                local.get 14
                f32.div
                local.set 13
                local.get 12
                local.get 14
                f32.div
                local.set 12
                local.get 11
                local.get 14
                f32.div
                local.set 11
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
              local.set 4
              local.get 4
              f32.load offset=8
              local.set 13
              local.get 4
              f32.load
              local.set 12
              local.get 4
              f32.load offset=4
              local.set 11
            end
            local.get 2
            local.get 13
            f32.store offset=72
            local.get 2
            local.get 2
            i32.load offset=72
            i32.store offset=40
            local.get 2
            local.get 11
            f32.store offset=68
            local.get 2
            local.get 12
            f32.store offset=64
            local.get 2
            i32.load offset=64
            local.set 5
            local.get 2
            i32.load offset=68
            local.set 7
            local.get 2
            local.get 5
            i32.store offset=32
            local.get 2
            local.get 7
            i32.store offset=36
            local.get 9
            local.get 3
            local.get 2
            i32.const 32
            i32.add
            i32.const 3473692
            i32.load
            call $j1293 ;; 
            local.get 3
            i32.const 1
            i32.add
            local.set 3
            br 1 (;@3;)
          end
        end
        i32.const 3435416
        i32.load
        call $j871 ;; 
        local.get 8
        i32.const 3495728
        i32.load
        call $j1764 ;; 
        local.set 3
        local.get 2
        local.get 3
        i32.store offset=124
        i32.const 3432484
        i32.load
        call $j871 ;; 
        local.get 3
        i32.const 3432484
        i32.load
        i32.load offset=92
        i32.load offset=100
        i32.const 0
        call $j8009 ;; 
        i32.const 3432484
        i32.load
        i32.load offset=92
        i32.load offset=116
        local.get 10
        local.get 2
        i32.load offset=124
        i32.const 3452656
        i32.load
        call $j1769 ;; 
      end
      local.get 0
      i32.load offset=416
      local.get 1
      i32.const 164
      i32.mul
      i32.add
      local.get 2
      i32.load offset=124
      i32.store offset=68
    end
    local.get 2
    i32.const 128
    i32.add
    global.set 0)
